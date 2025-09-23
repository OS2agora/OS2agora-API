namespace BallerupKommune.Operations.Authentication
{
    public class OAuth2TokenResponse
    {
        public string Access_Token { get; set; }
        public string Id_Token { get; set; }
        public string Refresh_Token { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public string Code { get; set; }
        public string Error { get; set; }
        public string Error_Description { get; set; }
        public string State { get; set; }
    }
}