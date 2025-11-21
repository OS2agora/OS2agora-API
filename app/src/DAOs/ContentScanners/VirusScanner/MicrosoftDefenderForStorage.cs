using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Agora.Operations.Common.Enums;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Exceptions;
using Azure.Identity;
using Azure.Storage.Blobs;
using Agora.Operations.ApplicationOptions;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using Azure.Core;


namespace Agora.DAOs.ContentScanners.VirusScanner
{
    public sealed class MicrosoftDefenderForStorage : IVirusScanner
    {
        private readonly IOptions<MicrosoftDefenderForStorageOptions> _microsoftDefenderForStorageOptions;
        private readonly IOptions<AzureOptions> _azureOptions;
        
        public MicrosoftDefenderForStorage(IOptions<MicrosoftDefenderForStorageOptions> microsoftDefenderForStorageOptions, IOptions<AzureOptions> azureOptions)
        {
            _microsoftDefenderForStorageOptions = microsoftDefenderForStorageOptions;
            _azureOptions = azureOptions;
        }

        Task<VirusScannerResult> IVirusScanner.ScanFileAsync(byte[] file)
        {
            return Task.Run(() => ScanFile(file));
        }

        private VirusScannerResult ScanFile(byte[] file)
        {
            string filename = GenerateFileName();
            BlobClient client = GetBlobClient(filename);

            UploadToMalwareScanner(client, file);
            try
            {
                return PollForScanningComplete(client);
            }
            finally
            {
                client.Delete();
            }
        }

        private void UploadToMalwareScanner(BlobClient client, byte[] file)
        {
            client.Upload(new BinaryData(file));
        }

        private VirusScannerResult PollForScanningComplete(BlobClient client)
        {
            int retries = _microsoftDefenderForStorageOptions.Value.ResultPollRetries;
            int msDelayPerRetry = _microsoftDefenderForStorageOptions.Value.ResultPollMsDelayPerRetry;
            for (int attempt = 1; attempt <= retries; attempt++)
            {
                GetBlobTagResult blobTagsResult = client.GetTags().Value;
                IDictionary<string, string> tags = blobTagsResult.Tags;

                if (tags.ContainsKey(_microsoftDefenderForStorageOptions.Value.MalwareScanKeyName))
                {
                    string tagResult = tags[_microsoftDefenderForStorageOptions.Value.MalwareScanKeyName];
                    if(tagResult == _microsoftDefenderForStorageOptions.Value.MalwareScanCleanValue)
                    {
                        return VirusScannerResult.Clean;
                    } 
                    else if(tagResult == _microsoftDefenderForStorageOptions.Value.MalwareScanMaliciousValue)
                    {
                        return VirusScannerResult.VirusDetected;
                    } 
                    else
                    {
                        return VirusScannerResult.Unknown;
                    }                    
                }
                Thread.Sleep(msDelayPerRetry);
            }
            return VirusScannerResult.Error;
        }
        
        private BlobClient GetBlobClient(string blobname)
        {
            // Validate configuration options:
            if (string.IsNullOrEmpty(_microsoftDefenderForStorageOptions.Value.ContainerName))
            {
                throw new GeneralException("ContainerName must be specified");
            }

            if (!String.IsNullOrWhiteSpace(_microsoftDefenderForStorageOptions.Value.StorageConnectionString))
            {
                // Credentials are specified as part of the connection string, so create a client based on that:
                return new BlobClient(_microsoftDefenderForStorageOptions.Value.StorageConnectionString, _microsoftDefenderForStorageOptions.Value.ContainerName, blobname);
            }
            else if (!String.IsNullOrWhiteSpace(_microsoftDefenderForStorageOptions.Value.StorageAccountUrl))
            {
                // No credentials specified in configuration, so get the Azure credentials, which should be the system assigned identity:
                var credentials = GetAzureCredentials();
                Uri blobUri = new Uri($"{_microsoftDefenderForStorageOptions.Value.StorageAccountUrl}/{_microsoftDefenderForStorageOptions.Value.ContainerName}/{blobname}");                
                return new BlobClient(blobUri, credentials);
            }
            else
            {
                throw new GeneralException("StorageConnectionString or StorageAccountUrl must be specified");
            }
        }

        private string GenerateFileName()
        {
            return Guid.NewGuid().ToString();
        }

        private TokenCredential GetAzureCredentials()
        {
            var managedIdentityClientId = _azureOptions.Value.ManagedIdentityClientId;
            if (!_azureOptions.Value.IsRunningInAzure || string.IsNullOrEmpty(managedIdentityClientId))
            {
                // Use default credentials if not running in azure or managed identity client id is not configured
                return new DefaultAzureCredential();
            }

            // Use managed identity for authentication
            return new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ManagedIdentityClientId = _azureOptions.Value.ManagedIdentityClientId
            });
        }

        


    }
}
