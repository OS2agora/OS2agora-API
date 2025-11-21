using Agora.Operations.Authentication;
using System.Threading.Tasks;
using Agora.Models.Enums;

namespace Agora.Operations.Common.Interfaces
{
    public interface IOauth2Service
    {
        string CreateAuthorizationUrl(string codeVerifier, string state, string apiKey, UserCapacity userCapacity = UserCapacity.NONE);
        string GetAuthenticationEndpoint();

        Task<OAuth2TokenResponse> RequestAccessTokenCode(string tokenRequestCode, string tokenRequestCodeVerifier, string apiKey, UserCapacity userCapacity = UserCapacity.NONE);
    }
}