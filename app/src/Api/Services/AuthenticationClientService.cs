using BallerupKommune.Operations.ApplicationOptions;
using BallerupKommune.Operations.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Text.RegularExpressions;
using OAuth2Client = BallerupKommune.Operations.ApplicationOptions.OAuth2Client;
using BallerupKommune.Operations.Common.Enums;

namespace BallerupKommune.Api.Services
{
    public class AuthenticationClientService : IAuthenticationClientService
    {
        private readonly IOptions<OAuth2Options> _oauth2Options;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationClientService(IOptions<OAuth2Options> oauth2Options,
            IHttpContextAccessor httpContextAccessor)
        {
            _oauth2Options = oauth2Options;
            _httpContextAccessor = httpContextAccessor;
        }

        public string ReadApiKey()
        {
            return _httpContextAccessor.HttpContext.Request.Headers["x-api-key"];
        }

        public bool IsApiKeyTrusted(string apiKey)
        {
            var trustedApiKey = _oauth2Options.Value.InternalApiKey == apiKey ||
                                _oauth2Options.Value.PublicApiKey == apiKey;

            return trustedApiKey;
        }

        public string GetApiKeyFromOptions(ClientTypes client)
        {
            switch (client) { 
                case ClientTypes.Public:
                    return _oauth2Options.Value.PublicApiKey;
                case ClientTypes.Internal:
                    return _oauth2Options.Value.InternalApiKey;
                default:
                    return null;
            }
        }

        public string GetAuthenticationEndpoint()
        {
            return _oauth2Options.Value.TokenEndpoint;
        }

        public OAuth2Client GetOAuthClientOptions(string apiKey)
        {
            if (IsApiKeyTrusted(apiKey))
            {
                var result = new OAuth2Client
                {
                    TokenEndpoint = _oauth2Options.Value.TokenEndpoint,
                    AuthorizeEndpoint = _oauth2Options.Value.AuthorizeEndpoint,
                    MaxAge = _oauth2Options.Value.MaxAge,
                    Scope = _oauth2Options.Value.Scope,
                };

                if (_oauth2Options.Value.InternalApiKey == apiKey)
                {
                    var internalClientSecret = _oauth2Options.Value.InternalClientSecret;

                    result.RedirectUri = _oauth2Options.Value.InternalRedirectUri;
                    result.ClientId = _oauth2Options.Value.InternalClientId;
                    result.ClientSecret = internalClientSecret;
                    result.ApiKey = _oauth2Options.Value.InternalApiKey;
                    result.Whr = _oauth2Options.Value.WhrInternal;
                }

                if (_oauth2Options.Value.PublicApiKey == apiKey)
                {
                    var publicClientSecret = _oauth2Options.Value.PublicClientSecret;

                    result.RedirectUri = _oauth2Options.Value.PublicRedirectUri;
                    result.ClientId = _oauth2Options.Value.PublicClientId;
                    result.ClientSecret = publicClientSecret;
                    result.ApiKey = _oauth2Options.Value.PublicApiKey;
                    result.Whr = _oauth2Options.Value.WhrPublic;
                }

                return result;
            }

            throw new Exception("Invalid api-key.");
        }
    }
}