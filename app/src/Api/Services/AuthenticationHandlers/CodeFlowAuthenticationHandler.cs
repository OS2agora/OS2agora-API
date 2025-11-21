using Agora.Api.Services.AuthenticationHandlers.utility;
using Agora.Api.Services.Interfaces;
using Agora.Models.Enums;
using Agora.Operations.Authentication;
using Agora.Operations.Common.Enums;
using Agora.Operations.Common.Interfaces;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Agora.Api.Services.AuthenticationHandlers
{
    public class CodeFlowAuthenticationHandler : BaseAuthenticationHandler
    {
        private readonly IOauth2Service _oauth2Service;
        private readonly IJwtService _jwtService;
        private readonly ICookieService _cookieService;
        private readonly IClaimsService<CodeFlowAuthenticationHandler> _claimsService;
        private readonly ILogger<CodeFlowAuthenticationHandler> _logger;

        public CodeFlowAuthenticationHandler(IOauth2Service oauth2Service, IJwtService jwtService, ICookieService cookieService, IClaimsService<CodeFlowAuthenticationHandler> claimsService, ILogger<CodeFlowAuthenticationHandler> logger)
        {
            _oauth2Service = oauth2Service;
            _jwtService = jwtService;
            _cookieService = cookieService;
            _claimsService = claimsService;
            _logger = logger;
        }

        public override AuthHandlerType Type => AuthHandlerType.CodeFlow;

        public override IActionResult HandleLogin(IHttpContextAccessor httpContextAccessor, string redirectUri, string loginAs, string apiKey, UserCapacity userCapacity)
        {
            using var activity = AuthenticationHandlerActivity(nameof(CodeFlowAuthenticationHandler));
            var codeVerifier = CryptoRandom.CreateUniqueId();
            var state = CryptoRandom.CreateUniqueId();
            var authorizationUrl = _oauth2Service.CreateAuthorizationUrl(codeVerifier, state, apiKey, userCapacity);
            var timestamp = DateTime.Now.ToString("O");

            if (!httpContextAccessor.HttpContext.Session.IsAvailable)
            {
                return new BadRequestObjectResult("Session unavailable");
            }

            httpContextAccessor.SetSessionValue(SessionKeys.CodeVerifier, codeVerifier);
            httpContextAccessor.SetSessionValue(SessionKeys.RedirectUri, redirectUri);
            httpContextAccessor.SetSessionValue(SessionKeys.State, state);
            httpContextAccessor.SetSessionValue(SessionKeys.Timestamp, timestamp);
            httpContextAccessor.SetSessionValue(SessionKeys.ApiKey, apiKey);
            httpContextAccessor.SetSessionValue(SessionKeys.LoginAs, loginAs);
            httpContextAccessor.SetSessionValue(SessionKeys.UserCapacity, Enum.GetName(typeof(UserCapacity), userCapacity));
            return new RedirectResult(authorizationUrl);
        }

        public override async Task<IActionResult> HandleLogout(IHttpContextAccessor httpContextAccessor, string redirectUri, string apiKey)
        {
            using var activity = AuthenticationHandlerActivity(nameof(CodeFlowAuthenticationHandler));
            await _jwtService.RevokeRefreshToken();
            _cookieService.RemoveAccessCookieInResponse(apiKey);
            return new RedirectResult(redirectUri);
        }

        public override async Task<IActionResult> HandleOAuthCallback(IHttpContextAccessor httpContextAccessor, string code, string state, Func<string, string, string, IActionResult> doLogin)
        {
            using var activity = AuthenticationHandlerActivity(nameof(CodeFlowAuthenticationHandler));
            var timeStampAsDateTime = httpContextAccessor.GetSessionValue(SessionKeys.Timestamp,
                (s) => DateTime.ParseExact(s, "O", CultureInfo.InvariantCulture));

            var apiKeyAsString = httpContextAccessor.GetSessionValue(SessionKeys.ApiKey);

            var notProduction = !Primitives.Logic.Environment.IsProduction();

            var maxTimeSinceLoginRequest = 100; // in seconds
            if (timeStampAsDateTime <= DateTime.Now.AddSeconds(-maxTimeSinceLoginRequest))
            {
                var redirectUriAsString = httpContextAccessor.GetSessionValue(SessionKeys.RedirectUri);
                var loginAs = httpContextAccessor.GetSessionValue(SessionKeys.LoginAs);
                return doLogin(redirectUriAsString, loginAs, notProduction ? apiKeyAsString : null);
            }

            var stateAsString = httpContextAccessor.GetSessionValue(SessionKeys.State);

            if (state != stateAsString)
            {
                var redirectUriAsString = httpContextAccessor.GetSessionValue(SessionKeys.RedirectUri);
                var loginAs = httpContextAccessor.GetSessionValue(SessionKeys.LoginAs);
                return doLogin(redirectUriAsString, loginAs, notProduction ? apiKeyAsString : null);
            }

            var userCapacityAsString = httpContextAccessor.GetSessionValue(SessionKeys.UserCapacity);
            var userCapacity = (UserCapacity)Enum.Parse(typeof(UserCapacity), userCapacityAsString, true);


            var codeVerifierAsString = httpContextAccessor.GetSessionValue(SessionKeys.CodeVerifier);

            // This function is not called directly by the frontend-applications
            // So the apiKey does not exist in a header, and we have to send it in as a parameter manually
            var tokenResponseFromIdp =
                await _oauth2Service.RequestAccessTokenCode(code, codeVerifierAsString, apiKeyAsString, userCapacity);

            var userReadFromToken = _jwtService.ReadAccessToken(tokenResponseFromIdp.Access_Token);

            var tokenUser = ParseUserFromClaims(userReadFromToken);

            var newJwtTokenPayload = await _jwtService.LoginAndGenerateAccessToken(tokenUser);

            // This function is not called directly by the frontend-applications
            // So the apiKey does not exist in a header, and we have to send it in as a parameter manually
            _cookieService.SetAccessCookieInResponse(newJwtTokenPayload.Token, apiKeyAsString);

            var redirectUrl = httpContextAccessor.GetSessionValue(SessionKeys.RedirectUri);

            httpContextAccessor.RemoveSessionValue(SessionKeys.CodeVerifier);
            httpContextAccessor.RemoveSessionValue(SessionKeys.RedirectUri);
            httpContextAccessor.RemoveSessionValue(SessionKeys.State);
            httpContextAccessor.RemoveSessionValue(SessionKeys.Timestamp);
            httpContextAccessor.RemoveSessionValue(SessionKeys.ApiKey);
            httpContextAccessor.RemoveSessionValue(SessionKeys.LoginAs);
            httpContextAccessor.RemoveSessionValue(SessionKeys.UserCapacity);

            return new RedirectResult(redirectUrl);
        }

        public override TokenUser ParseUserFromClaims(ClaimsPrincipal token)
        {
            using var activity = AuthenticationHandlerActivity(nameof(CodeFlowAuthenticationHandler));

            var tokenUser = new TokenUser
            {
                Cpr = token.FindFirstValue(_claimsService.GetCprNumberIdentifier()),
                Cvr = token.FindFirstValue(_claimsService.GetCvrNumberIdentifier()),
                Email = token.FindFirstValue(_claimsService.GetEmail()),
                Pid = token.FindFirstValue(_claimsService.GetPidNumberIdentifier()),
                CompanyName = token.FindFirstValue(_claimsService.GetCompanyNameIdentifier())
            };

            SetAuthenticationMethod(tokenUser);
            SetEmployeeDisplayName(tokenUser, token);
            SetPersonalIdentifier(tokenUser);
            SetName(tokenUser, token);
            SetPossibleRoles(tokenUser, token);

            return tokenUser;
        }

        private static void SetAuthenticationMethod(TokenUser tokenUser)
        {
            if (tokenUser.Email != null)
            {
                tokenUser.AuthMethod = AuthenticationMethod.AdfsEmployee;
            }
            else if (tokenUser.Cvr != null)
            {
                tokenUser.AuthMethod = AuthenticationMethod.MitIdErhverv;
            }
            else
            {
                tokenUser.AuthMethod = AuthenticationMethod.MitIdCitizen;
            }
        }

        private void SetPersonalIdentifier(TokenUser tokenUser)
        {
            switch (tokenUser.AuthMethod)
            {
                case AuthenticationMethod.MitIdErhverv:
                    tokenUser.PersonalIdentifier = $"{tokenUser.Cvr}-{tokenUser.Cpr}";
                    break;
                case AuthenticationMethod.MitIdCitizen:
                    tokenUser.PersonalIdentifier = tokenUser.Cpr;
                    break;
                case AuthenticationMethod.AdfsEmployee:
                    tokenUser.PersonalIdentifier = tokenUser.Email;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tokenUser.AuthMethod), tokenUser.AuthMethod, null);
            }
            ValidatePersonalIdentifier(tokenUser);
        }

        private void ValidatePersonalIdentifier(TokenUser tokenUser)
        {
            if (string.IsNullOrEmpty(tokenUser.PersonalIdentifier))
            {
                _logger.LogError("An error occured while parsing personal identifier - information missing for authentication method {authenticationMethod}", 
                    Enum.GetName(typeof(AuthenticationMethod), tokenUser.AuthMethod));
                throw new ArgumentOutOfRangeException(nameof(tokenUser.PersonalIdentifier),
                    tokenUser.PersonalIdentifier, null);
            }
        }

        private void SetName(TokenUser tokenUser, ClaimsPrincipal token)
        {
            if (tokenUser.AuthMethod == AuthenticationMethod.AdfsEmployee)
            {
                var firstName = token.FindFirstValue(_claimsService.GetFirstName());
                var lastName = token.FindFirstValue(_claimsService.GetLastName());
                tokenUser.Name = $"{firstName} {lastName}";
            }
            else
            {
                tokenUser.Name = token.FindFirstValue(_claimsService.GetFullName());
            }
        }

        private void SetEmployeeDisplayName(TokenUser tokenUser, ClaimsPrincipal token)
        {
            if (tokenUser.AuthMethod == AuthenticationMethod.AdfsEmployee)
            {
                 tokenUser.EmployeeDisplayName = token.FindFirstValue(_claimsService.GetDisplayName());
            }
        }

        private void SetPossibleRoles(TokenUser tokenUser, ClaimsPrincipal token)
        {
            if (tokenUser.AuthMethod == AuthenticationMethod.AdfsEmployee)
            {
                tokenUser.PossibleRoles = token.FindAll(_claimsService.GetRoles()).ToList();
            }
        }
    }
}
