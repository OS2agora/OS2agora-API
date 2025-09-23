namespace BallerupKommune.Operations.ApplicationOptions
{
    public class JwtSettingsOptions
    {
        public const string JwtSettings = "JwtSettings";
        public const string SecretSubSection = "Secret";

        public string Secret { get; set; }
        public string SecondsToAccessTokenExpiration { get; set; }
        public string SecondsToRefreshTokenExpiration { get; set; }
    }
}