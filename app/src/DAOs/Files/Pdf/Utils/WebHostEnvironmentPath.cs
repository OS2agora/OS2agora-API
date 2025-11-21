using Agora.Operations.Common.Interfaces.Files.Pdf;
using Microsoft.AspNetCore.Hosting;

namespace Agora.DAOs.Files.Pdf.Utils
{
    public class WebHostEnvironmentPath : IHostingEnvironmentPath
    {
        public string WebRootPath { get; }

        public WebHostEnvironmentPath(IWebHostEnvironment webHostEnvironment)
        {
            WebRootPath = webHostEnvironment.WebRootPath;
        }
    }
}
