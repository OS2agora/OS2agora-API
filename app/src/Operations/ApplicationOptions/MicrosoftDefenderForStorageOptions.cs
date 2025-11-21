using System;
using System.Collections.Generic;
using System.Text;

namespace Agora.Operations.ApplicationOptions
{
    public class MicrosoftDefenderForStorageOptions
    {
        public const string MicrosoftDefenderForStorage = "MicrosoftDefenderForStorage";

        public bool Enabled { get; set; } = false;
        /// <summary>
        /// The Key on the tag that Microsoft Defender uses for reporting result of malware scanning.
        /// </summary>
        public string MalwareScanKeyName { get; set; } = "Malware Scanning scan result";
        /// <summary>
        /// The value that Microsoft Defender uses for indicating that a file contains malware/virus.
        /// </summary>
        public string MalwareScanMaliciousValue { get; set; } = "Malicious";
        /// <summary>
        /// The value that Microsoft Defender uses for indicating that a file is clean.
        /// </summary>
        public string MalwareScanCleanValue { get; set; }  = "No threats found";
        /// <summary>
        /// The storage account blob URL - e.g. https://mystorageaccount.blob.core.windows.net. Use this to authenticate with Azure identity. This must be set when the ConnectionString is NOT set.
        /// </summary>
        public string StorageAccountUrl { get; set; }
        // Use for accessing external storage account (not managed identity): 
        /// <summary>
        /// Connection string for storage account in the form: DefaultEndpointsProtocol=https;AccountName=<name>;AccountKey=<account-key>. Use this to authenticate with a storage account access key. This must be set if StorageAccountUrl is NOT set.
        /// </summary>
        public string StorageConnectionString { get; set; }
        /// <summary>
        /// Name of container in the storage account to use for malware scanning.
        /// </summary>
        public string ContainerName { get; set; } = "for-scanning";

        public int ResultPollRetries { get; set; } = 120;
        public int ResultPollMsDelayPerRetry { get; set; } = 1000;

    }
}
