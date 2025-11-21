using Agora.Models.Models;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using MediatR;
using NovaSec.Attributes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;
using ContentType = Agora.Models.Enums.ContentType;
using Agora.Operations.Common.Interfaces.Files;

namespace Agora.Operations.Models.Contents.Queries.DownloadFiles
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.HearingId)")]
    public class DownloadFilesQuery : IRequest<FileDownload>
    {
        public int HearingId { get; set; }

        public class DownloadFilesQueryHandler : IRequestHandler<DownloadFilesQuery, FileDownload>
        {
            private readonly IFileService _fileService;
            private readonly IHearingDao _hearingDao;
            private readonly IContentDao _contentDao;

            public DownloadFilesQueryHandler(IFileService fileService, IHearingDao hearingDao, IContentDao contentDao)
            {
                _fileService = fileService;
                _hearingDao = hearingDao;
                _contentDao = contentDao;
            }

            public async Task<FileDownload> Handle(DownloadFilesQuery request, CancellationToken cancellationToken)
            {
                Hearing currentHearing = await _hearingDao.GetAsync(request.HearingId);
                if (currentHearing == null)
                {
                    throw new NotFoundException(nameof(Hearing), request.HearingId);
                }

                var contentIncludes = IncludeProperties.Create<Content>(null, new List<string>
                {
                    nameof(Content.Comment),
                    nameof(Content.ContentType)
                });
                List<Content> contentFromCommentsWithFileContent = await _contentDao.GetAllAsync(contentIncludes,
                    content => content.Comment.HearingId == request.HearingId &&
                               content.ContentType.Type == ContentType.FILE);

                var result = await _fileService.ZipContent(contentFromCommentsWithFileContent);
                return new FileDownload
                {
                    ContentType = "application/zip",
                    FileName = "Høringssvar_bilag.zip",
                    Content = result
                };
            }
        }
    }
}