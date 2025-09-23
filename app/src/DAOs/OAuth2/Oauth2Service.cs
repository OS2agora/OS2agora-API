using BallerupKommune.Models.Extensions;
using BallerupKommune.Operations.Common.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BallerupKommune.Operations.Authentication;

namespace BallerupKommune.DAOs.OAuth2
{
    public class Oauth2Service : IOauth2Service
    {
        private readonly OAuth2Client _oAuth2Client;
        private readonly IAuthenticationClientService _authenticationClientService;
        private readonly ILogger<Oauth2Service> _logger;

        public Oauth2Service(OAuth2Client oAuth2Client, IAuthenticationClientService authenticationClientService, ILogger<Oauth2Service> logger)
        {
            _oAuth2Client = oAuth2Client;
            _authenticationClientService = authenticationClientService;
            _logger = logger;
        }

        public string CreateAuthorizationUrl(string codeVerifier, string state, string apiKey)
        {
            var oauthClientOptions = _authenticationClientService.GetOAuthClientOptions(apiKey);

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

            var codeChallenge = codeVerifier;
            var codeChallengeMethod = "plain";

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

        public async Task<OAuth2TokenResponse> RequestAccessTokenCode(string tokenRequestCode,
            string tokenRequestCodeVerifier, string apiKey)
        {
            var oauthClientOptions = _authenticationClientService.GetOAuthClientOptions(apiKey);

            return await _oAuth2Client.RequestAccessTokenCode(tokenRequestCode, tokenRequestCodeVerifier,
                oauthClientOptions);
        }
    }
}