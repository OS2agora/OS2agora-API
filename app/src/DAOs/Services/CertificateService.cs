using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Interfaces;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Microsoft.Extensions.Options;

namespace Agora.DAOs.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly IOptions<AzureOptions> _azureOptions;

        public CertificateService(IOptions<AzureOptions> azureOptions)
        {
            _azureOptions = azureOptions;
        }


        public X509Certificate2 GetPrivateCertificateFromKeyVault(string keyVaultRef)
        {
            if (string.IsNullOrEmpty(keyVaultRef))
            {
                throw new ArgumentException($"Missing configuration for certificate - cannot find query KeyVault using ref: {keyVaultRef}");
            }

            if (string.IsNullOrEmpty(_azureOptions.Value.ManagedIdentityClientId))
            {
                throw new ArgumentException("Missing configuration: 'Azure:ManagedIdentityClientId'");
            }

            var azureCredentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ManagedIdentityClientId = _azureOptions.Value.ManagedIdentityClientId
            });

            var client = new CertificateClient(new Uri(_azureOptions.Value.KeyVaultUrl), azureCredentials);
            var certificate = client.DownloadCertificate(keyVaultRef);
            return certificate.Value;
        }

        public X509Certificate2 GetPrivateCertificateFromDisc(string filePath, string password)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Certificate file not found at '{filePath}'. The current Directory is '{Directory.GetCurrentDirectory()}'.");
            }

            return new X509Certificate2(filePath, password: password);
        }
    }
}