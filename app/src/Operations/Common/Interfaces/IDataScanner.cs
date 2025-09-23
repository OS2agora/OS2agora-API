using BallerupKommune.Operations.Common.Enums;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Common.Interfaces
{
    public interface IDataScanner
    {
        Task<DataScannerResult> ScanFileContentAsync(byte[] file, string mimeType, string fileName);
        Task<DataScannerResult> ScanTextContentAsync(string textContent);
    }
}