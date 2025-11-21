using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Agora.Operations.Common.Enums;
using Agora.Operations.Common.Interfaces;

namespace Agora.DAOs.ContentScanners.DataScanner
{
    /// <summary>
    /// A simple data scanner implementation that only validates text-content using simple regex patterns.
    /// Beware of false positives and false negatives when using this scanner!
    /// </summary>
    public class SimpleDataScanner : IDataScanner
    {
        private static readonly string CprRegEx = @"\b(\d{6})(?:[ \-\/\.\t]|[ ]\-[ ])?(\d{4})\b";

        public Task<DataScannerResult> ScanFileContentAsync(byte[] file, string mimeType, string fileName)
        {
            return Task.FromResult(DataScannerResult.Clean);
        }

        public Task<DataScannerResult> ScanTextContentAsync(string textContent)
        {
            var result = Regex.Matches(textContent, CprRegEx);

            if (result.Any())
            {
                return Task.FromResult(DataScannerResult.Dirty);
            }

            return Task.FromResult(DataScannerResult.Clean);
        }
    }
}