using System.Security.Cryptography.X509Certificates;

namespace Agora.Operations.Common.Interfaces
{
    public interface ICertificateService
    {
        public X509Certificate2 GetPrivateCertificateFromKeyVault(string keyVaultRef);
        public X509Certificate2 GetPrivateCertificateFromDisc(string filePath, string password);
    }
}