using BallerupKommune.Operations.Common.Enums;
using BallerupKommune.Operations.Common.Interfaces;
using nClam;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.ContentScanners.VirusScanner
{
    public sealed class VirusScanner : IVirusScanner
    {
        private readonly ClamClient _clamClient;

        public VirusScanner(ClamClient clamClient)
        {
            _clamClient = clamClient;
        }

        public async Task<VirusScannerResult> ScanFileAsync(byte[] file)
        {
            var scanResult = await _clamClient.SendAndScanFileAsync(file);
            return MapClamScanResult(scanResult);
        }

        private VirusScannerResult MapClamScanResult(ClamScanResult clamScanResult)
        {
            return clamScanResult.Result switch
            {
                ClamScanResults.Clean => VirusScannerResult.Clean,
                ClamScanResults.VirusDetected => VirusScannerResult.VirusDetected,
                ClamScanResults.Error => VirusScannerResult.Error,
                _ => VirusScannerResult.Unknown,
            };
        }
    }
}
