using Agora.DAOs.ExternalProcesses.Enums;
using Agora.DAOs.ExternalProcesses.Interfaces;

namespace Agora.ExternalProcessHandler.Models
{
    public class ExternalProcessConfiguration : IExternalProcessConfiguration
    {
        public ExternalProcessJobs Job { get; set; }
    }
}

