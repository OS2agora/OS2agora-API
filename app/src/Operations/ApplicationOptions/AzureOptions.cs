namespace Agora.Operations.ApplicationOptions
{
    /// <summary>
    /// Options relevant for integrating with the azure hosting environment
    /// </summary>
    public class AzureOptions
    {
        public const string Azure = "Azure";

        public bool IsRunningInAzure { get; set; } = false;

        public string ManagedIdentityClientId { get; set; } = "";
        public string ManagedIdentityObjectId { get; set; } = "";

        public string KeyVaultUrl { get; set; } = "";
        public bool UseAzureDbAuth { get; set; } = false;
    }
}