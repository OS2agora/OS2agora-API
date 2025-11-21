using Agora.Operations.Common.Interfaces.Files.Pdf;

namespace Agora.ExternalProcessHandler.Models
{
    public class ExternalProcessPath : IHostingEnvironmentPath
    {
        public string WebRootPath { get; set; }
    }
}