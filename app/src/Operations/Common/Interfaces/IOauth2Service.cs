using BallerupKommune.Operations.Authentication;
using System.Threading.Tasks;

namespace BallerupKommune.Operations.Common.Interfaces
{
    public interface IOauth2Service
    {
        string CreateAuthorizationUrl(string codeVerifier, string state, string apiKey);

        Task<OAuth2TokenResponse> RequestAccessTokenCode(string tokenRequestCode, string tokenRequestCodeVerifier, string apiKey);
    }
}