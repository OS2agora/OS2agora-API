using BallerupKommune.Operations.Common.Enums;
using BallerupKommune.Operations.Common.Interfaces;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.ContentScanners.DataScanner
{
    public class DataScanner : IDataScanner
    {
        private readonly DataScannerClient _dataScannerClient;

        public DataScanner(DataScannerClient dataScannerClient)
        {
            _dataScannerClient = dataScannerClient;
        }

        public Task<DataScannerResult> ScanFileContentAsync(byte[] file, string mimeType, string fileName)
        {
            return _dataScannerClient.ScanFileContent(file, mimeType, fileName);
        }

        public Task<DataScannerResult> ScanTextContentAsync(string textContent)
        {
            return _dataScannerClient.ScanTextContent(textContent);
        }
    }
}