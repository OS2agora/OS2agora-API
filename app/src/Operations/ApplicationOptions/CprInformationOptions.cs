namespace Agora.Operations.ApplicationOptions
{
    public class CprInformationOptions
    {
        public const string CprInformation = "CprInformation";

        public string CertificatePath { get; set; }
        public string CertificateKeyVaultReference { get; set; }
        public string CertificatePassword { get; set; }
        public string MunicipalityCvr { get; set; }
        public string Endpoint { get; set; }
        public bool Enable { get; set; }
    }
}