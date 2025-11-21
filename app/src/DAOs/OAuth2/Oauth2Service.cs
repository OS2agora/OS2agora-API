using Agora.Models.Extensions;
using Agora.Operations.Common.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Agora.Operations.Authentication;
using Agora.Operations.ApplicationOptions;
using Microsoft.Extensions.Options;
using UserCapacity = Agora.Models.Enums.UserCapacity;

namespace Agora.DAOs.OAuth2
{
    public class Oauth2Service : IOauth2Service
    {
        private readonly OAuth2Client _oAuth2Client;
        private readonly IAuthenticationClientService _authenticationClientService;
        private readonly ILogger<Oauth2Service> _logger;
        private readonly IOptions<OAuth2Options> _oauth2Options;
        private readonly IOptions<AuthenticationOptions> _authOptions;

        public Oauth2Service(OAuth2Client oAuth2Client, IAuthenticationClientService authenticationClientService, ILogger<Oauth2Service> logger, IOptions<OAuth2Options> oauth2Options, IOptions<AuthenticationOptions> authOptions)
        {
            _oAuth2Client = oAuth2Client;
            _authenticationClientService = authenticationClientService;
            _logger = logger;
            _oauth2Options = oauth2Options;
            _authOptions = authOptions;
        }

        public string CreateAuthorizationUrl(string codeVerifier, string state, string apiKey, UserCapacity userCapacity = UserCapacity.NONE)
        {
            var oauthClientOptions = GetOAuthClientOptions(apiKey, userCapacity);

            var authorizationEndpoint = oauthClientOptions.AuthorizeEndpoint;
            var clientId = oauthClientOptions.ClientId;
            var scope = oauthClientOptions.Scope;
            var redirectUri = oauthClientOptions.RedirectUri;
            var responseType = "code".UrlEncode();
            var nonce = Guid.NewGuid().ToString("N");
            var maxAge = oauthClientOptions.MaxAge;
            var whr = oauthClientOptions.Whr;

            var returnUrl =
                $"{authorizationEndpoint}?client_id={clientId.UrlEncode()}&scope={scope.UrlEncode()}&redirect_uri={redirectUri.UrlEncode()}&response_type={responseType}&state={state.UrlEncode()}";

            var codeChallenge = GenerateCodeChallenge(codeVerifier);
            var codeChallengeMethod = "S256";

            returnUrl =
                $"{returnUrl}&code_challenge={codeChallenge.UrlEncode()}&code_challenge_method={codeChallengeMethod.UrlEncode()}";

            var claimsIdentify = new ClaimsIdentity();
            claimsIdentify.AddClaim(new Claim("client_id", clientId));
            claimsIdentify.AddClaim(new Claim("response_type", "code"));
            claimsIdentify.AddClaim(new Claim("scope", scope));
            claimsIdentify.AddClaim(new Claim("redirect_uri", redirectUri));
            claimsIdentify.AddClaim(new Claim("max_age", maxAge));
            claimsIdentify.AddClaim(new Claim("state", state));
            claimsIdentify.AddClaim(new Claim("nonce", nonce));
            claimsIdentify.AddClaim(new Claim("whr", whr));

            var jwtSecurityToken =
                new JwtSecurityTokenHandler().CreateJwtSecurityToken(clientId, authorizationEndpoint, claimsIdentify);
            var token = "eyJhbGciOiJub25lIn0" + "." + jwtSecurityToken.EncodedPayload + ".";

            returnUrl += "&request=" + token;

            return returnUrl;
        }

        public async Task<OAuth2TokenResponse> RequestAccessTokenCode(
            string tokenRequestCode,
            string tokenRequestCodeVerifier, 
            string apiKey, 
            UserCapacity userCapacity)
        {
            var oauthClientOptions = GetOAuthClientOptions(apiKey, userCapacity);

            return await _oAuth2Client.RequestAccessTokenCode(tokenRequestCode, tokenRequestCodeVerifier,
                oauthClientOptions);
        }
        public string GetAuthenticationEndpoint()
        {
            return _oauth2Options.Value.TokenEndpoint;
        }

        /// <summary>
        /// Create a SHA256 hash of the code verifier and convert to base64 without padding
        /// </summary>
        /// <param name="codeVerifier"></param>
        /// <returns></returns>
        private string GenerateCodeChallenge(string codeVerifier)
        {
            using var sha256 = SHA256.Create();
            var challengeBytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier));
            var base64 = Convert.ToBase64String(challengeBytes).Replace("/", "_").Replace("+", "-").TrimEnd('=');
            return base64;
        }

        private OAuth2ClientOptions GetOAuthClientOptions(string apiKey, UserCapacity userCapacity = UserCapacity.NONE)
        {
            if (!_authenticationClientService.IsApiKeyTrusted(apiKey))
            {
                throw new Exception("Invalid api-key.");
            }

            OAuth2Options capacityOverrides = userCapacity switch
            {
                UserCapacity.EMPLOYEE => _oauth2Options.Value.EmployeeOverrides,
                UserCapacity.CITIZEN => _oauth2Options.Value.CitizenAuthOverrides,
                UserCapacity.COMPANY => _oauth2Options.Value.CompanyOverrides,
                _ => null
            };

            var result = new OAuth2ClientOptions
            {
                TokenEndpoint = capacityOverrides?.TokenEndpoint ?? _oauth2Options.Value.TokenEndpoint,
                AuthorizeEndpoint = capacityOverrides?.AuthorizeEndpoint ?? _oauth2Options.Value.AuthorizeEndpoint,
                MaxAge = capacityOverrides?.MaxAge ?? _oauth2Options.Value.MaxAge,
                Scope = capacityOverrides?.Scope ?? _oauth2Options.Value.Scope,
            };

            if (_authOptions.Value.InternalApiKey == apiKey)
            {
                result.RedirectUri = capacityOverrides?.InternalRedirectUri ?? _oauth2Options.Value.InternalRedirectUri;
                result.ClientId = capacityOverrides?.InternalClientId ?? _oauth2Options.Value.InternalClientId;
                result.ClientSecret = capacityOverrides?.InternalClientSecret ?? _oauth2Options.Value.InternalClientSecret;
                result.ApiKey = _authOptions.Value.InternalApiKey;
                result.Whr = capacityOverrides?.WhrInternal ?? _oauth2Options.Value.WhrInternal;
            } 
            else if (_authOptions.Value.PublicApiKey == apiKey)
            {
                result.RedirectUri = capacityOverrides?.PublicRedirectUri ?? _oauth2Options.Value.PublicRedirectUri;
                result.ClientId = capacityOverrides?.PublicClientId ?? _oauth2Options.Value.PublicClientId;
                result.ClientSecret = capacityOverrides?.PublicClientSecret ?? _oauth2Options.Value.PublicClientSecret;
                result.ApiKey = _authOptions.Value.PublicApiKey;
                result.Whr = capacityOverrides?.WhrPublic ?? _oauth2Options.Value.WhrPublic;
            }

            return result;
        }


    }
}