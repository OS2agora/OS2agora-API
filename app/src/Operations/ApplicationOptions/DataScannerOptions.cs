namespace Agora.Operations.ApplicationOptions
{
    public class DataScannerOptions
    {
        public const string DataScanner = "DataScanner";

        public string BaseAddress { get; set; }
        public string Token { get; set; }
        public bool UseSimpleDataScanner { get; set; }
    }
}