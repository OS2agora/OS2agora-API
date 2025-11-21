using Agora.Api.Services.AuthenticationHandlers.utility;
using Agora.Api.Services.Interfaces;
using Agora.Operations.Authentication;
using Agora.Operations.Common.Enums;
using Agora.Operations.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Agora.Models.Enums;
using Agora.Operations.ApplicationOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Agora.Api.Services.AuthenticationHandlers
{
    public class EntraIdAuthenticationHandler : BaseAuthenticationHandler
    {
        private readonly IJwtService _jwtService;
        private readonly ICookieService _cookieService;
        private readonly IAuthenticationClientService _authenticationClientService;
        private readonly IClaimsService<EntraIdAuthenticationHandler> _claimsService;
        private readonly IOptions<EntraIdOptions> _options;
        private readonly ILogger<EntraIdAuthenticationHandler> _logger;

        public EntraIdAuthenticationHandler(IClaimsService<EntraIdAuthenticationHandler> claimsService, IJwtService jwtService, ICookieService cookieService, IAuthenticationClientService authenticationClientService, IOptions<EntraIdOptions> options, ILogger<EntraIdAuthenticationHandler> logger)
        {
            _claimsService = claimsService;
            _jwtService = jwtService;
            _cookieService = cookieService;
            _authenticationClientService = authenticationClientService;
            _options = options;
            _logger = logger;
        }


        public override AuthHandlerType Type => AuthHandlerType.EntraId;

        private static List<string> Scopes => new List<string> { "openid", "profile", "email" };

        private string GetAuthCallbackUri(string apiKey)
        {
            var clientType = _authenticationClientService.GetClientTypeFromApiKey(apiKey);

            switch (clientType)
            {
                case ClientTypes.Public:
                    return _options.Value.PublicRedirectUri;
                case ClientTypes.Internal:
                    return _options.Value.InternalRedirectUri;
                default:
                    _logger.LogError("Failed to determine entraId callback url to use for entraIdAuthHandler");
                    throw new Exception("Failed to determine ClientType");
            }
        }

        private IConfidentialClientApplication CreateConfidentialClientApplication(string apiKey)
        {
            var callbackUri = GetAuthCallbackUri(apiKey);

            var options = _options.Value;

            var clientAppOptions = new ConfidentialClientApplicationOptions
            {
                ClientId = options.ClientId,
                Instance = options.Instance,
                TenantId = options.TenantId,
                ClientSecret = options.ClientSecret,
                RedirectUri = callbackUri
            };

            return ConfidentialClientApplicationBuilder.CreateWithApplicationOptions(clientAppOptions).Build();
        }

        public override IActionResult HandleLogin(IHttpContextAccessor httpContextAccessor, string redirectUri, string loginAs, string apiKey, UserCapacity userCapacity)
        {
            using var activity = AuthenticationHandlerActivity(nameof(EntraIdAuthenticationHandler));
            var timestamp = DateTime.Now.ToString("O");

            var confidentialClientApplication = CreateConfidentialClientApplication(apiKey);

            var authUrl = confidentialClientApplication.GetAuthorizationRequestUrl(Scopes)
                .WithPkce(out var codeVerifier)
                .ExecuteAsync().GetAwaiter().GetResult();

            httpContextAccessor.SetSessionValue(SessionKeys.CodeVerifier, codeVerifier);
            httpContextAccessor.SetSessionValue(SessionKeys.RedirectUri, redirectUri);
            httpContextAccessor.SetSessionValue(SessionKeys.Timestamp, timestamp);
            httpContextAccessor.SetSessionValue(SessionKeys.ApiKey, apiKey);
            httpContextAccessor.SetSessionValue(SessionKeys.LoginAs, loginAs);

            return new RedirectResult(authUrl.ToString());
        }

        public override async Task<IActionResult> HandleEntraIdCallback(IHttpContextAccessor httpContextAccessor, string code, string state, Func<string, string, string, IActionResult> doLogin)
        {
            using var activity = AuthenticationHandlerActivity(nameof(EntraIdAuthenticationHandler));
            var timeStampAsDateTime = httpContextAccessor.GetSessionValue(SessionKeys.Timestamp,
                (s) => DateTime.ParseExact(s, "O", CultureInfo.InvariantCulture));

            var apiKeyAsString = httpContextAccessor.GetSessionValue(SessionKeys.ApiKey);
            var codeVerifierAsString = httpContextAccessor.GetSessionValue(SessionKeys.CodeVerifier);
            var redirectUriAsString = httpContextAccessor.GetSessionValue(SessionKeys.RedirectUri);

            var maxTimeSinceLoginRequest = 100; // in seconds
            if (timeStampAsDateTime <= DateTime.Now.AddSeconds(-maxTimeSinceLoginRequest))
            {
                var loginAs = httpContextAccessor.GetSessionValue(SessionKeys.LoginAs);
                return doLogin(redirectUriAsString, loginAs, Primitives.Logic.Environment.IsProduction() ? null : apiKeyAsString);
            }
            
            var confidentialClientApplication = CreateConfidentialClientApplication(apiKeyAsString);
            var token = await confidentialClientApplication.AcquireTokenByAuthorizationCode(Scopes, code).WithPkceCodeVerifier(codeVerifierAsString)
                .ExecuteAsync();

            var tokenUser = ParseUserFromClaims(token.ClaimsPrincipal);

            var newJwtTokenPayload = await _jwtService.LoginAndGenerateAccessToken(tokenUser);

            // This function is not called directly by the frontend-applications
            // So the apiKey does not exist in a header, and we have to send it in as a parameter manually
            _cookieService.SetAccessCookieInResponse(newJwtTokenPayload.Token, apiKeyAsString);

            httpContextAccessor.RemoveSessionValue(SessionKeys.CodeVerifier);
            httpContextAccessor.RemoveSessionValue(SessionKeys.RedirectUri);
            httpContextAccessor.RemoveSessionValue(SessionKeys.Timestamp);
            httpContextAccessor.RemoveSessionValue(SessionKeys.ApiKey);
            httpContextAccessor.RemoveSessionValue(SessionKeys.LoginAs);

            return new RedirectResult(redirectUriAsString);
        }

        public override async Task<IActionResult> HandleLogout(IHttpContextAccessor httpContextAccessor, string redirectUri, string apiKey)
        {
            using var activity = AuthenticationHandlerActivity(nameof(EntraIdAuthenticationHandler));
            await _jwtService.RevokeRefreshToken();
            _cookieService.RemoveAccessCookieInResponse(apiKey);
            return new RedirectResult(redirectUri);
        }

        public override TokenUser ParseUserFromClaims(ClaimsPrincipal token)
        {
            using var activity = AuthenticationHandlerActivity(nameof(EntraIdAuthenticationHandler));
            return new TokenUser
            {
                AuthMethod = AuthenticationMethod.AdfsEmployee,
                EmployeeDisplayName = token.FindFirstValue(_claimsService.GetDisplayName()),
                PersonalIdentifier = token.FindFirstValue(_claimsService.GetUniqueIdentifier()),
                Name = token.FindFirstValue(_claimsService.GetFullName()),
                PossibleRoles = token.FindAll(_claimsService.GetRoles()).ToList(),
                Email = token.FindFirstValue(_claimsService.GetEmail())
            };
        }
    }
}