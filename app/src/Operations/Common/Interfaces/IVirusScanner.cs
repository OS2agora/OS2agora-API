using Agora.Operations.Common.Enums;
using System.Threading.Tasks;

namespace Agora.Operations.Common.Interfaces
{
    public interface IVirusScanner
    {
        Task<VirusScannerResult> ScanFileAsync(byte[] file);
    }
}
