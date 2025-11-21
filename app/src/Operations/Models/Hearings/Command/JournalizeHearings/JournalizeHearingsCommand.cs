using Agora.Models.Models;
using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Operations.Common.Extensions;
using Agora.Operations.Resolvers;
using CommentStatus = Agora.Models.Enums.CommentStatus;
using CommentType = Agora.Models.Enums.CommentType;
using ContentType = Agora.Models.Enums.ContentType;
using FieldType = Agora.Models.Enums.FieldType;
using HearingStatus = Agora.Models.Enums.HearingStatus;
using JournalizedStatus = Agora.Models.Enums.JournalizedStatus;
using Agora.Models.Models.Records;
using Agora.Operations.Common.Interfaces.Files.Pdf;
using Agora.Operations.Common.Interfaces.Files;

namespace Agora.Operations.Models.Hearings.Command.JournalizeHearings
{
    public class JournalizeHearingsCommand : IRequest<Unit>
    {
        public class JournalizeHearingsCommandHandler : IRequestHandler<JournalizeHearingsCommand, Unit>
        {
            private readonly IHearingDao _hearingDao;
            private readonly IJournalizeStatusDao _journalizeStatusDao;
            private readonly IEsdhService _esdhService;
            private readonly IPdfService _pdfService;
            private readonly IFileService _fileService;
            private readonly IFieldSystemResolver _fieldSystemResolver;
            private readonly ILogger<JournalizeHearingsCommandHandler> _logger;

            public JournalizeHearingsCommandHandler(IHearingDao hearingDao, IJournalizeStatusDao journalizeStatusDao, IEsdhService esdhService,
                IPdfService pdfService, IFieldSystemResolver fieldSystemResolver,
                IFileService fileService, ILogger<JournalizeHearingsCommandHandler> logger)
            {
                _hearingDao = hearingDao;
                _journalizeStatusDao = journalizeStatusDao;
                _esdhService = esdhService;
                _fileService = fileService;
                _fieldSystemResolver = fieldSystemResolver;
                _logger = logger;
                _pdfService = pdfService;
            }

            public async Task<Unit> Handle(JournalizeHearingsCommand request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<Hearing>(null, new List<string>
                {
                    $"{nameof(Hearing.Contents)}",
                    $"{nameof(Hearing.Contents)}.{nameof(Content.ContentType)}",
                    $"{nameof(Hearing.Comments)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.CommentType)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.CommentStatus)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.Contents)}.{nameof(Content.ContentType)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.User)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.User)}.{nameof(User.UserCapacity)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.User)}.{nameof(User.Company)}",
                    $"{nameof(Hearing.SubjectArea)}",
                });

                var journalizedStatuses = await _journalizeStatusDao.GetAllAsync();
                var journalized = journalizedStatuses.Single(status => status.Status == JournalizedStatus.JOURNALIZED);
                var notJournalized = journalizedStatuses.Single(status => status.Status == JournalizedStatus.NOT_JOURNALIZED);
                var journalizedWithError = journalizedStatuses.Single(status => status.Status == JournalizedStatus.JOURNALIZED_WITH_ERROR);

                List<Hearing> hearingsToJournalize = await _hearingDao.GetAllAsync(includes, hearing =>
                    hearing.HearingStatus.Status == HearingStatus.CONCLUDED && hearing.JournalizedStatusId == notJournalized.Id);

                _logger.LogInformation($"Hearings to journalize: {hearingsToJournalize.Count}");

                foreach (Hearing hearing in hearingsToJournalize)
                {
                    _logger.LogInformation($"Journalizing hearing with id: {hearing.Id}");
                    try
                    {
                        await JournalizeHearing(hearing);
                    }
                    catch (Exception e)
                    {
                        // Continue to let other hearings try to be journalized
                        _logger.LogError(
                            $"Exception caught when trying to journalize hearing with id: {hearing.Id}. Error: {e.Message}");
                        hearing.JournalizedStatusId = journalizedWithError.Id;
                        await _hearingDao.UpdateAsync(hearing);
                        continue;
                    }
                    hearing.JournalizedStatusId = journalized.Id;
                    await _hearingDao.UpdateAsync(hearing);
                }

                return Unit.Value;
            }

            private async Task JournalizeHearing(Hearing hearing)
            {
                Content titleContent = await hearing.GetTextContentOfFieldType(_fieldSystemResolver, FieldType.TITLE);
                Content summaryContent = await hearing.GetTextContentOfFieldType(_fieldSystemResolver, FieldType.SUMMARY);
                Content conclusionContent = await hearing.GetTextContentOfFieldType(_fieldSystemResolver, FieldType.CONCLUSION);
                Content bodyInformationTextContent = await hearing.GetTextContentOfFieldType(_fieldSystemResolver, FieldType.BODYINFORMATION);
                List<Content> bodyInformationFiles = await hearing.GetFileContentsOfFieldType(_fieldSystemResolver, FieldType.BODYINFORMATION);

                if (titleContent == null || summaryContent == null || bodyInformationTextContent == null || conclusionContent == null ||
                    hearing.HearingType?.Name == null || hearing.SubjectArea?.Name == null ||
                    !hearing.StartDate.HasValue ||
                    !hearing.Deadline.HasValue)
                {
                    _logger.LogWarning($"Hearing with ID {hearing.Id} is concluded but is missing content. Will be set as Journalized.");
                    return;
                }

                var hearingRecord = new HearingRecord
                {
                    BaseData = new HearingBaseData
                    {
                        Title = titleContent.TextContent,
                        Summary = summaryContent.TextContent,
                        BodyText = bodyInformationTextContent.TextContent,
                        Conclusion = conclusionContent.TextContent,
                        ConclusionCreatedDate = conclusionContent.Created,
                        EsdhNumber = hearing.EsdhNumber,
                        SubjectArea = hearing.SubjectArea.Name,
                        HearingType = hearing.HearingType.Name,
                        StartDate = hearing.StartDate.Value,
                        Deadline = hearing.Deadline.Value,

                    }
                };

                var hearingPdf = _pdfService.CreateHearingPdf(hearingRecord);
                var conclusionPdf = _pdfService.CreateConclusionPdf(hearingRecord);

                var hearingId = hearing.Id;

                await _esdhService.JournalizeHearingText(hearingId, hearingPdf, Esdh.HearingFileName, Esdh.HearingFileDescription, Esdh.HearingFileContentType);
                await _esdhService.JournalizeHearingConclusion(hearing.Id, conclusionPdf, Esdh.ConclusionFileName, Esdh.ConclusionFileDescription, Esdh.ConclusionFileContentType);

                foreach (var file in bodyInformationFiles)
                {
                    var fileFromDisk = await _fileService.GetFileFromDisk(file.FilePath);
                    var fileNameInEsdh = Esdh.HearingAppendixTitle(file.FileName);
                    await _esdhService.JournalizeHearingText(hearingId, fileFromDisk, fileNameInEsdh, Esdh.HearingAppendixDescription, file.FileContentType);
                }

                var commentsOnHearing = hearing.Comments.Where(x => x.CommentType.Type == CommentType.HEARING_RESPONSE && !x.IsDeleted && x.CommentStatus.Status == CommentStatus.APPROVED);

                foreach (var comment in commentsOnHearing)
                {
                    var commentTextContent = comment.Contents.SingleOrDefault(x => x.ContentType.Type == ContentType.TEXT);

                    if (commentTextContent == null)
                    {
                        continue;
                    }

                    // If comment is created after the hearing deadline, then use the hearing deadline as the created date.
                    var clampedCreatedDate = (!hearing.Deadline.HasValue || comment.Created < hearing.Deadline) ? comment.Created : hearing.Deadline.Value;

                    var commentRecord = new CommentRecord
                    {
                        CommentText = commentTextContent.TextContent,
                        Created = clampedCreatedDate,
                        Number = comment.Number,
                        ResponderName = comment.User.UserCapacity.Capacity == Agora.Models.Enums.UserCapacity.COMPANY ? comment.User.Company.Name : comment.User.Name,
                    };

                    var commentPdf = _pdfService.CreateCommentPdf(commentRecord, hearingRecord);

                    var commentFileName = Esdh.CommentFileName(commentRecord.Number);
                    var commentFileDescription = Esdh.CommentFileDescription(commentRecord.Number, commentRecord.ResponderName);

                    await _esdhService.JournalizeHearingAnswer(hearingId, commentPdf, commentFileName, commentFileDescription, Esdh.CommentFileContentType);

                    var filesOnComment = comment.Contents.Where(x => x.ContentType.Type == ContentType.FILE);

                    foreach (var file in filesOnComment)
                    {
                        var fileFromDisk = await _fileService.GetFileFromDisk(file.FilePath);

                        var fileNameInEsdh = Esdh.CommentAppendixTitle(commentRecord.Number, file.FileName);
                        var fileDescriptionInEsdh = Esdh.CommentAppendixDescription(commentRecord.Number);

                        await _esdhService.JournalizeHearingAnswer(hearingId, fileFromDisk, fileNameInEsdh, fileDescriptionInEsdh, file.FileContentType);
                    }
                }

                await _esdhService.CloseCase(hearingId);
            }
        }
    }
}