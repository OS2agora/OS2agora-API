using BallerupKommune.Models.Enums;
using BallerupKommune.Models.Models;
using BallerupKommune.Models.Models.Records;
using BallerupKommune.Operations.Common.Interfaces;
using BallerupKommune.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BallerupKommune.Models.Common;
using BallerupKommune.Operations.Common.Extensions;
using BallerupKommune.Operations.Resolvers;
using CommentStatus = BallerupKommune.Models.Enums.CommentStatus;
using CommentType = BallerupKommune.Models.Enums.CommentType;
using ContentType = BallerupKommune.Models.Enums.ContentType;
using FieldType = BallerupKommune.Models.Enums.FieldType;

namespace BallerupKommune.Operations.Models.Hearings.Queries.ExportHearing
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.Id)")]
    public class ExportHearingQuery : IRequest<FileDownload>
    {
        public int Id { get; set; }
        public ExportFormat Format { get; set; }

        public class ExportHearingsQueryHandler : IRequestHandler<ExportHearingQuery, FileDownload>
        {
            private readonly IHearingDao _hearingDao;
            private readonly IFieldSystemResolver _fieldSystemResolver;
            private readonly IPdfService _pdfService;
            private readonly IExcelService _excelService;

            public ExportHearingsQueryHandler(IHearingDao hearingDao, IFieldSystemResolver fieldSystemResolver,
              IPdfService pdfService, IExcelService excelService)
            {
                _hearingDao = hearingDao;
                _fieldSystemResolver = fieldSystemResolver;
                _pdfService = pdfService;
                _excelService = excelService;
            }

            public async Task<FileDownload> Handle(ExportHearingQuery request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<Hearing>(null, new List<string>
                {
                    $"{nameof(Hearing.Contents)}",
                    $"{nameof(Hearing.Contents)}.{nameof(Content.ContentType)}",
                    $"{nameof(Hearing.Contents)}.{nameof(Content.Field)}.{nameof(Field.FieldType)}",
                    $"{nameof(Hearing.Comments)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.User)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.CommentType)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.CommentStatus)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.Contents)}",
                    $"{nameof(Hearing.Comments)}.{nameof(Comment.Contents)}.{nameof(Content.ContentType)}",
                    $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.HearingRole)}",
                    $"{nameof(Hearing.UserHearingRoles)}.{nameof(UserHearingRole.User)}",
                });

                var hearing = await _hearingDao.GetAsync(request.Id, includes);

                User hearingOwner = hearing.UserHearingRoles?.SingleOrDefault(x => x.HearingRole?.Role == BallerupKommune.Models.Enums.HearingRole.HEARING_OWNER)?.User;
                var hearingOwnerDisplayName = hearingOwner?.EmployeeDisplayName;

                var titleContent = await hearing.GetTextContentOfFieldType(_fieldSystemResolver, FieldType.TITLE);
                var title = "Agora_BALK_Eksport_" + titleContent.TextContent + "_" + System.DateTime.Now.ToString();
                var esdhNumber = hearing.EsdhNumber;

                Comment[] commentsOnHearing = hearing.Comments
                  .Where(comment =>
                    comment.CommentType.Type != CommentType.HEARING_REVIEW ||
                    comment.CommentStatus.Status != CommentStatus.NONE)
                  .ToArray();

                var commentRecords = new List<CommentRecord>();
                foreach (var comment in commentsOnHearing)
                {
                    if (comment.CommentType.Type == CommentType.HEARING_RESPONSE_REPLY)
                    {
                        continue;
                    }
                    Content commentTextContent = comment.Contents.SingleOrDefault(content =>
                      content.ContentType.Type == ContentType.TEXT);

                    if (commentTextContent == null)
                    {
                        commentTextContent = new Content { TextContent = "Tom kommentar" };
                    }

                    if (comment.CommentParrentId != null)
                    {
                        comment.Number += 1;
                    }

                    var commentText = GenerateComment(comment, commentTextContent, commentsOnHearing);

                    var filesOnComment =
                      comment.Contents.Where(content => content.ContentType.Type == ContentType.FILE);
                    var fileNamesOnComment = filesOnComment.Select(content => content.FileName).ToList();
                    var commentAnswersToComment = commentsOnHearing.Where(localComment =>
                      localComment.CommentParrentId == comment.Id &&
                      localComment.CommentType.Type == CommentType.HEARING_RESPONSE_REPLY &&
                      !localComment.IsDeleted &&
                      localComment.Contents.Any(y => !string.IsNullOrEmpty(y.TextContent)));

                    var commentAnswersAsText = commentAnswersToComment
                      .Select(localComment =>
                        localComment.Contents.SingleOrDefault(content =>
                          content.ContentType.Type == ContentType.TEXT))
                      .Where(content => content != null).Select(content => content.TextContent).ToList();

                    commentRecords.Add(new CommentRecord
                    {
                        Id = comment.Number,
                        Comment = commentText,
                        OnBehalfOf = comment.OnBehalfOf,
                        CommentDeclineInfo = comment.CommentDeclineInfo,
                        HearingOwnerDisplayName = hearingOwnerDisplayName,
                        AnswersToComment = commentAnswersAsText,
                        FileNames = fileNamesOnComment
                    });
                }

                switch (request.Format)
                {
                    case ExportFormat.PDF:
                        {
                            var pdfContent = _pdfService.CreateHearingReport(title, esdhNumber, commentRecords);
                            return new FileDownload
                            {
                                ContentType = "application/pdf",
                                FileName = $"Rapport: {title}.pdf",
                                Content = pdfContent
                            };
                        }
                    case ExportFormat.EXCEL:
                        var excelContent = _excelService.CreateHearingReport(commentRecords);
                        return new FileDownload
                        {
                            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            FileName = $"Rapport: {title}.xlsx",
                            Content = excelContent
                        };
                    default:
                        return new FileDownload();
                }
            }


            private static string GenerateComment(Comment comment, Content commentTextContent, IEnumerable<Comment> commentsOnHearing)
            {
                var commentText = string.Empty;

                if (comment.User != null)
                {
                    commentText = "Kommentar fra " + (comment.User.EmployeeDisplayName ?? comment.User.Name) + " " ?? "";

                    if (!string.IsNullOrEmpty(comment.User.EmployeeDisplayName ?? comment.User.Name) && !string.IsNullOrEmpty(comment.OnBehalfOf))
                    {
                        commentText += "på vegne af " + comment.OnBehalfOf + " ";
                    }
                }

                var commentStatus = comment.CommentStatus.Status;
                commentText += AddCommentStatus(commentStatus);
                commentText += commentTextContent.TextContent;

                return commentText;
            }

            private static string AddCommentStatus(CommentStatus status)
            {
                var statusString = "";
                switch (status)
                {
                    case CommentStatus.NOT_APPROVED:
                        statusString += " (Afvist høringssvar): ";
                        break;
                    case CommentStatus.APPROVED:
                        statusString += " (Godkendt høringssvar): ";
                        break;
                    case CommentStatus.AWAITING_APPROVAL:
                    case CommentStatus.NONE:
                        statusString += " (Ikke godkendt høringssvar): ";
                        break;
                }

                return statusString;
            }
        }
    }
}