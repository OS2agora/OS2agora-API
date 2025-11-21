using Agora.Api.Services.Interfaces;
using Agora.Operations.Authentication;
using Agora.Operations.Common.Enums;
using Agora.Operations.Common.Interfaces;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Agora.Models.Enums;

namespace Agora.Api.Services.AuthenticationHandlers
{
    public class NemLoginAuthenticationHandler : BaseAuthenticationHandler
    {
        private const string RelayStateReturnUrl = "ReturnUrl";
        private const string RelayStateApiKey = "ApiKey";

        private readonly IJwtService _jwtService;
        private readonly ICookieService _cookieService;
        private readonly IClaimsService<NemLoginAuthenticationHandler> _claimsService;
        private readonly ILogger<NemLoginAuthenticationHandler> _logger;
        private readonly Saml2Configuration _saml2Configuration;

        public NemLoginAuthenticationHandler(IJwtService jwtService, ICookieService cookieService, IClaimsService<NemLoginAuthenticationHandler> claimsService, ILogger<NemLoginAuthenticationHandler> logger, Saml2Configuration saml2Configuration)
        {
            _jwtService = jwtService;
            _cookieService = cookieService;
            _claimsService = claimsService;
            _logger = logger;
            _saml2Configuration = saml2Configuration;
        }

        public override AuthHandlerType Type => AuthHandlerType.NemLogin;

        public override IActionResult HandleLogin(IHttpContextAccessor httpContextAccessor, string redirectUri, string loginAs, string apiKey, UserCapacity userCapacity)
        {
            using var activity = AuthenticationHandlerActivity(nameof(NemLoginAuthenticationHandler));
            // Create SAML redirect binding and redirect user to login page
            var binding = new Saml2RedirectBinding();
            binding.SetRelayStateQuery(new Dictionary<string, string>
            {
                { RelayStateReturnUrl, redirectUri },
                { RelayStateApiKey, apiKey }
            });

            var request = new Saml2AuthnRequest(_saml2Configuration);

            _logger.LogInformation("Binding AuthnRequest with Id: {Id}", request.IdAsString);

            binding = binding.Bind(request);

            return new RedirectResult(binding.RedirectLocation.OriginalString);
        }

        public override async Task<IActionResult> HandleLogout(IHttpContextAccessor httpContextAccessor, string redirectUri, string apiKey)
        {
            using var activity = AuthenticationHandlerActivity(nameof(NemLoginAuthenticationHandler));
            await _jwtService.RevokeRefreshToken();
            _cookieService.RemoveAccessCookieInResponse(apiKey);

            // Do not initialize logout flow if user is not authenticated
            if (httpContextAccessor.HttpContext.User.Identity is { IsAuthenticated: false })
            {
                return new RedirectResult(redirectUri);
            }

            var binding = new Saml2RedirectBinding();
            binding.SetRelayStateQuery(new Dictionary<string, string>
            {
                { RelayStateReturnUrl, redirectUri },
                { RelayStateApiKey, apiKey }
            });

            var saml2LogoutRequest = new Saml2LogoutRequest(_saml2Configuration, httpContextAccessor.HttpContext.User);

            _logger.LogInformation("User triggered SAML single logout RequestId: {RequestId} NameId: {NameId} SessionIndex: {SessionIndex}", saml2LogoutRequest.IdAsString, saml2LogoutRequest.NameId.Value, saml2LogoutRequest.SessionIndex);
            
            return binding.Bind(saml2LogoutRequest).ToActionResult();
        }
        public override IActionResult HandleNemLoginLogoutResponse(IHttpContextAccessor httpContextAccessor,
            string defaultRedirectUrl)
        {
            using var activity = AuthenticationHandlerActivity(nameof(NemLoginAuthenticationHandler));
            var httpRequest = httpContextAccessor.HttpContext.Request.ToGenericHttpRequest(validate: true);
            httpRequest.Binding.Unbind(httpRequest, new Saml2LogoutResponse(_saml2Configuration));

            // Parse relay state
            var relayStateQuery = httpRequest.Binding.GetRelayStateQuery();
            var returnUrl = relayStateQuery.ContainsKey(RelayStateReturnUrl) ? relayStateQuery[RelayStateReturnUrl] : defaultRedirectUrl;

            return new RedirectResult(returnUrl);
        }

        public override async Task<IActionResult> HandleNemLoginSingleLogout(IHttpContextAccessor httpContextAccessor,
            string defaultRedirectUrl, string apiKey = null)
        {
            using var activity = AuthenticationHandlerActivity(nameof(NemLoginAuthenticationHandler));
            // Parse and assert SAML Response
            var httpRequest = httpContextAccessor.HttpContext.Request.ToGenericHttpRequest(validate: true);
            var logoutRequest = new Saml2LogoutRequest(_saml2Configuration, httpContextAccessor.HttpContext.User);

            // Parse relay state
            var relayStateQuery = httpRequest.Binding.GetRelayStateQuery();
            apiKey = relayStateQuery.ContainsKey(RelayStateApiKey) ? relayStateQuery[RelayStateApiKey] : apiKey;

            Saml2StatusCodes status;

            try
            {
                httpRequest.Binding.Unbind(httpRequest, logoutRequest);
                status = Saml2StatusCodes.Success;
                await _jwtService.RevokeRefreshToken();
                _cookieService.RemoveAccessCookieInResponse(apiKey);
            }
            catch (Exception e)
            {
                // log exception
                _logger.LogWarning("SingleLogout error: {Message} RequestId: {RequestId}", e.ToString(), logoutRequest.IdAsString);
                status = Saml2StatusCodes.RequestDenied;
            }

            _logger.LogInformation("Single logout was triggered by another application. RequestId: {RequestId} NameId: {NameId} SessionIndex: {SessionIndex} Status: {Status}", logoutRequest.IdAsString, logoutRequest.NameId.Value, logoutRequest.SessionIndex, status.ToString());

            var responsebinding = new Saml2RedirectBinding();
            responsebinding.RelayState = httpRequest.Binding.RelayState;
            var saml2LogoutResponse = new Saml2LogoutResponse(_saml2Configuration)
            {
                InResponseToAsString = logoutRequest.IdAsString,
                Status = status,
            };
            return responsebinding.Bind(saml2LogoutResponse).ToActionResult();
        }


        public override async Task<IActionResult> HandleNemLoginCallback(IHttpContextAccessor httpContextAccessor, string defaultRedirectUrl)
        {
            using var activity = AuthenticationHandlerActivity(nameof(NemLoginAuthenticationHandler));
            // Parse and assert SAML Response
            var httpRequest = httpContextAccessor.HttpContext.Request.ToGenericHttpRequest(validate: true);

            var binding = new Saml2PostBinding();
            var saml2AuthnResponse = new Saml2AuthnResponse(_saml2Configuration);

            binding.ReadSamlResponse(httpRequest, saml2AuthnResponse);

            if (saml2AuthnResponse.Status != Saml2StatusCodes.Success)
            {
                throw new AuthenticationException($"SAML Response status: {saml2AuthnResponse.Status}");
            }
            binding.Unbind(httpRequest, saml2AuthnResponse);

            // Parse relay state
            var relayStateQuery = binding.GetRelayStateQuery();
            var returnUrl = relayStateQuery.ContainsKey(RelayStateReturnUrl) ? relayStateQuery[RelayStateReturnUrl] : defaultRedirectUrl;
            var apiKey = relayStateQuery.ContainsKey(RelayStateApiKey) ? relayStateQuery[RelayStateApiKey] : null;

            // Extract claims, generate access token and login user
            var claimsPrincipal = new ClaimsPrincipal(saml2AuthnResponse.ClaimsIdentity);

            var tokenUser = ParseUserFromClaims(claimsPrincipal);

            // We have to enrich the user's token with these claims for the NemLogin-IdP to identify the user's session during logout.
            var nameIdClaimName = "http://schemas.itfoxtec.com/ws/2014/02/identity/claims/saml2nameid";
            var nameIdClaim = claimsPrincipal.FindFirst(nameIdClaimName);

            var sessionIdClaimName = "http://schemas.itfoxtec.com/ws/2014/02/identity/claims/saml2sessionindex";
            var sessionIdClaim = claimsPrincipal.FindFirst(sessionIdClaimName);

            tokenUser.AdditionalClaims.AddRange(new List<Claim>{nameIdClaim, sessionIdClaim});

            var tokenPayload = await _jwtService.LoginAndGenerateAccessToken(tokenUser);

            // This function is not called directly by the frontend-applications
            // So the apiKey does not exist in a header, and we have to send it in as a parameter manually
            _cookieService.SetAccessCookieInResponse(tokenPayload.Token, apiKey);

            // The following information must be logged in accordance to NemLogin specification 
            _logger.LogInformation("Received SAML AuthnResponse with RequestId: {RequestId} InResponseTo: {InResponseTo} NameId: {NameId} LOA: {LOA} ApplicationUserId: {ApplicationUserId}", 
                saml2AuthnResponse.IdAsString, 
                saml2AuthnResponse.InResponseToAsString, 
                saml2AuthnResponse.NameId.Value, 
                claimsPrincipal.FindFirstValue("https://data.gov.dk/concept/core/nsis/loa"), 
                tokenPayload.ApplicationUserId);

            return new RedirectResult(returnUrl);
        }

        public override TokenUser ParseUserFromClaims(ClaimsPrincipal token)
        {
            using var activity = AuthenticationHandlerActivity(nameof(NemLoginAuthenticationHandler));
            var tokenUser = new TokenUser
            {
                Cvr = token.FindFirstValue(_claimsService.GetCvrNumberIdentifier()),
                Cpr = token.FindFirstValue(_claimsService.GetCprNumberIdentifier()),
                Name = token.FindFirstValue(_claimsService.GetFullName()),
                EmployeeDisplayName = null,
                PossibleRoles = new List<Claim>(),
                Email = null,
                CompanyName = token.FindFirstValue(_claimsService.GetOrganizationName())
            };

            SetAuthenticationMethod(tokenUser);
            SetPersonalIdentifier(tokenUser);

            return tokenUser;
        }

        private static void SetAuthenticationMethod(TokenUser tokenUser)
        {
            if (tokenUser.Cvr != null)
            {
                tokenUser.AuthMethod = AuthenticationMethod.MitIdErhverv;
            }
            else if (tokenUser.Cpr != null)
            {
                tokenUser.AuthMethod = AuthenticationMethod.MitIdCitizen;
            }
            else
            {
                throw new ArgumentException("Cannot determine authentication method for NemLogin");
            }
        }

        private static void SetPersonalIdentifier(TokenUser tokenUser)
        {
            switch (tokenUser.AuthMethod)
            {
                case AuthenticationMethod.MitIdErhverv:
                    tokenUser.PersonalIdentifier = $"{tokenUser.Cvr}-{tokenUser.Cpr}";
                    return;
                case AuthenticationMethod.MitIdCitizen:
                    tokenUser.PersonalIdentifier = tokenUser.Cpr;
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tokenUser.AuthMethod), tokenUser.AuthMethod, null);
            }
        }
    }
}