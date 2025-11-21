namespace Agora.Operations.ApplicationOptions
{
    public class EntraIdOptions
    {
        public const string EntraId = "EntraId";

        public string Instance { get; set; }
        public string Domain { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string PublicRedirectUri { get; set; }
        public string InternalRedirectUri { get; set; }
    }
}