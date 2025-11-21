namespace Agora.Operations.ApplicationOptions
{
    public class CvrInformationOptions
    {
        public static string CvrInformation = "CvrInformation";

        public string CertificatePath { get; set; }
        public string CertificateKeyVaultReference { get; set; }
        public string CertificatePassword { get; set; }
        public string HostName { get; set; }

        public bool Enable { get; set; }
    }
}