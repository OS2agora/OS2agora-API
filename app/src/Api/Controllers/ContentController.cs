using Agora.Operations.Models.Contents.Queries.DownloadFile;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Agora.Operations.Models.Contents.Queries.DownloadFiles;

namespace Agora.Api.Controllers
{
    public class ContentController : ApiController
    {
        [HttpGet("{contentId}/download")]
        public async Task<FileResult> Download(int contentId)
        {
            var command = new DownloadFileQuery
            {
                Id = contentId,
            };

            var result = await Mediator.Send(command);

            return File(result.Content, result.ContentType, result.FileName);
        }

        [HttpGet("{hearingId}/downloadAll")]
        public async Task<FileResult> DownloadAll(int hearingId)
        {
            var command = new DownloadFilesQuery
            {
                HearingId = hearingId
            };

            var result = await Mediator.Send(command);

            return File(result.Content, result.ContentType, result.FileName);
        }
    }
}
