using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Security;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Operations.Common.Interfaces.Files;

namespace Agora.Operations.Models.Contents.Queries.DownloadFile
{
    public class DownloadFileQuery : IRequest<FileDownload>
    {
        public int Id { get; set; }

        public class DownloadFileQueryHandler : IRequestHandler<DownloadFileQuery, FileDownload>
        {
            private readonly IFileService _fileService;
            private readonly IContentDao _contentDao;
            private readonly ISecurityExpressions _securityExpressions;

            public DownloadFileQueryHandler(IFileService fileService, IContentDao contentDao,
                ISecurityExpressions securityExpressions)
            {
                _fileService = fileService;
                _contentDao = contentDao;
                _securityExpressions = securityExpressions;
            }

            public async Task<FileDownload> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<Content>(null,
                    new List<string>
                    {
                        nameof(Content.Hearing),
                        $"{nameof(Content.Hearing)}.{nameof(Hearing.HearingStatus)}",
                        nameof(Content.Comment),
                        $"{nameof(Content.Comment)}.{nameof(Comment.CommentType)}",
                        $"{nameof(Content.Comment)}.{nameof(Comment.CommentStatus)}",
                        $"{nameof(Content.Comment)}.{nameof(Comment.User)}"
                    });
                var currentContent = await _contentDao.GetAsync(request.Id, includes);

                if (currentContent == null)
                {
                    throw new NotFoundException(nameof(Content), request.Id);
                }

                var canSeeHearing = false;
                var isHearingOwner = false;
                var ownsComment = false;
                var approvedComment = false;

                if (currentContent.Hearing != null)
                {
                    canSeeHearing = _securityExpressions.CanSeeHearing(currentContent.Hearing.Id);
                    isHearingOwner = _securityExpressions.IsHearingOwnerByHearingId(currentContent.Hearing.Id);
                }

                if (currentContent.Comment != null)
                {
                    ownsComment = _securityExpressions.IsCommentOwner(currentContent.Comment);
                    approvedComment = _securityExpressions.IsCommentApproved(currentContent.Comment);
                }


                if (currentContent.Hearing != null && !canSeeHearing)
                {
                    throw new Exception("You do not have access to this content");
                }

                if (currentContent.Comment != null &&
                    !(ownsComment || isHearingOwner || approvedComment && canSeeHearing))
                {
                    throw new Exception("You do not have access to this comment");
                }

                if (currentContent.FilePath == null || currentContent.FileName == null ||
                    currentContent.FileContentType == null)
                {
                    throw new Exception("Content missing information");
                }

                var fileContent = await _fileService.GetFileFromDisk(currentContent.FilePath);

                return new FileDownload
                {
                    ContentType = currentContent.FileContentType,
                    Content = fileContent,
                    FileName = currentContent.FileName
                };
            }
        }
    }
}