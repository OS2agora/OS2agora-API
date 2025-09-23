using BallerupKommune.Operations.Common.Enums;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Common.Interfaces
{
    public interface IVirusScanner
    {
        Task<VirusScannerResult> ScanFileAsync(byte[] file);
    }
}
