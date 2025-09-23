using BallerupKommune.Operations.ApplicationOptions;
using BallerupKommune.Operations.Common.Enums;

namespace BallerupKommune.Operations.Common.Interfaces
{
    public interface IAuthenticationClientService
    {
        public bool IsApiKeyTrusted(string apiKey);
        OAuth2Client GetOAuthClientOptions(string apiKey);
        string ReadApiKey();
        string GetApiKeyFromOptions(ClientTypes client);
        string GetAuthenticationEndpoint();
    }
}