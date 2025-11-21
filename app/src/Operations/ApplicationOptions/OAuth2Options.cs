namespace Agora.Operations.ApplicationOptions
{
    public class OAuth2Options
    {
        public const string OAuth2 = "OAuth2";

        public string InternalClientId { get; set; }
        public string PublicClientId { get; set; }
        public string InternalClientSecret { get; set; }
        public string PublicClientSecret { get; set; }
        public string AuthorizeEndpoint { get; set; }
        public string TokenEndpoint { get; set; }
        public string Scope { get; set; }
        public string InternalRedirectUri { get; set; }
        public string PublicRedirectUri { get; set; }
        public string MaxAge { get; set; }
        public string WhrInternal { get; set; }
        public string WhrPublic { get; set; }

        // Some municipalities require multiple CodeFlow authentication handlers. Therefore, we must be able to configure overrides for these settings
        public OAuth2Options CitizenAuthOverrides { get; set; }
        public OAuth2Options EmployeeOverrides { get; set; }
        public OAuth2Options CompanyOverrides { get; set; }
    }

    public class OAuth2ClientOptions
    {
        public string ApiKey { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AuthorizeEndpoint { get; set; }
        public string TokenEndpoint { get; set; }
        public string Scope { get; set; }
        public string RedirectUri { get; set; }
        public string MaxAge { get; set; }
        public string Whr { get; set; }
    }
}