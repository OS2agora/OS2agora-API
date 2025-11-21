using Agora.DAOs.ExternalProcesses.Enums;

namespace Agora.DAOs.ExternalProcesses.Interfaces
{
    public interface IExternalProcessConfiguration
    {
        ExternalProcessJobs Job { get; set; }
    }
}
