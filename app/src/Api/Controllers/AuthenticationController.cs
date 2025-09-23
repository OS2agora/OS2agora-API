using BallerupKommune.Api.Models.JsonApi;
using BallerupKommune.DTOs.Models;
using BallerupKommune.Models.Models;
using BallerupKommune.Operations.Common.Enums;
using BallerupKommune.Operations.Common.Interfaces;
using BallerupKommune.Operations.Models.Users.Queries.GetMe;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UserDto = BallerupKommune.Api.Models.DTOs.UserDto;

namespace BallerupKommune.Api.Controllers
{
    public class AuthenticationController : ApiController
    {
        private readonly IJwtService _jwtService;
        private readonly ICookieService _cookieService;
        private readonly IOauth2Service _oauth2Service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticationClientService _authenticationClientService;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly ICurrentUserService _currentUserService;

        public AuthenticationController(IJwtService jwtService, ICookieService cookieService,
            IOauth2Service oauth2Service, IHttpContextAccessor httpContextAccessor,
            IAuthenticationClientService authenticationClientService, ILogger<AuthenticationController> logger, ICurrentUserService currentUserService)
        {
            _cookieService = cookieService;
            _oauth2Service = oauth2Service;
            _httpContextAccessor = httpContextAccessor;
            _authenticationClientService = authenticationClientService;
            _logger = logger;
            _currentUserService = currentUserService;
            _jwtService = jwtService;
        }

        [HttpGet("Authorized")]
        public IActionResult Authorized()
        {
            var isAuthenticated = IsAuthenticated(_httpContextAccessor?.HttpContext?.User);
            return Ok(isAuthenticated);
        }

        [HttpGet("RefreshToken")]
        public ActionResult<RefreshTokenDto> GetRefreshToken()
        {
            var refreshToken = _currentUserService.RefreshToken;
            var accessTokenExpiration = _currentUserService.ExpirationDate;

            if (refreshToken == null || accessTokenExpiration == default)
            {
                return BadRequest("Refresh token not valid");
            }

            var urlEncodedToken = HttpUtility.UrlEncode(refreshToken, Encoding.ASCII);

            var result = new RefreshTokenDto
            {
                Token = urlEncodedToken,
                AccessTokenExpirationDate = accessTokenExpiration
            };

            return Ok(result);
        }

        [HttpPost("RenewAccessToken")]
        public async Task<ActionResult<RefreshTokenDto>> RenewAccessToken([FromQuery] string refreshToken)
        {
            var apiKey = _authenticationClientService.ReadApiKey();
            var currentAccessToken = _cookieService.ReadAccessCookieFromRequest(apiKey);

            // Update access token
            var newAccessToken = await _jwtService.RenewAccessToken(currentAccessToken, refreshToken);

            // Set new access token in cookie
            _cookieService.SetAccessCookieInResponse(newAccessToken, apiKey);
            
            return Ok();
        }

        [Authorize]
        [HttpGet("Me")]
        public async Task<ActionResult<JsonApiTopLevelDto<UserDto>>> Me()
        {
            var user = await Mediator.Send(new GetMeQuery());

            var userDto = Mapper.Map<User, DTOs.Models.UserDto>(user);
            return Ok(userDto);
        }

        [HttpGet("GetAccessToken")]
        public async Task<IActionResult> GetAccessToken(ClientTypes client, string username)
        {
            if (Primitives.Logic.Environment.IsProduction())
            {
                return StatusCode(403);
            }

            if ( client == ClientTypes.None ) return BadRequest("Request does not contain valid Client. Use 'client=1' for public or 'client=2' for internal");

            if ( string.IsNullOrEmpty(username) ) return BadRequest("Request does not contain a username");

            string apiKey = _authenticationClientService.GetApiKeyFromOptions(client);
            if (string.IsNullOrEmpty(apiKey))
            {
                return BadRequest("Could not identify the provided client");
            }

            try
            {
                var tokenResponseFromIdp =
                await _oauth2Service.RequestAccessTokenCode(username, "codeVerifier", apiKey);
                var userReadFromToken = _jwtService.ReadAccessToken(tokenResponseFromIdp.Access_Token);
                var newJwtTokenPayload = await _jwtService.ExchangeExternalToken(userReadFromToken);

                _cookieService.SetAccessCookieInResponse(newJwtTokenPayload.Token, apiKey);
                return Ok(newJwtTokenPayload.Token);

            } catch (Exception e)
            {
                var AuthURL = new Uri(_authenticationClientService.GetAuthenticationEndpoint());
                return BadRequest($"Could not find user with the provided username. Find all allowed users in the dropdown here: {AuthURL.Scheme}://localhost:{AuthURL.Port}");
            }
        }


        [HttpGet("CodeFlowLogin")]
        public IActionResult CodeFlowLogin(string redirectUri, string apiKey = null)
        {
            if (Primitives.Logic.Environment.IsProduction() && !string.IsNullOrEmpty(apiKey))
            {
                return BadRequest("api-key only allowed in development");
            }

            apiKey ??= _authenticationClientService.ReadApiKey();

            var trustedApiKey = _authenticationClientService.IsApiKeyTrusted(apiKey);
            if (!trustedApiKey)
            {
                return BadRequest($"api-key: {apiKey} not valid");
            }

            var codeVerifier = CryptoRandom.CreateUniqueId();
            var state = CryptoRandom.CreateUniqueId();
            var authorizationUrl = _oauth2Service.CreateAuthorizationUrl(codeVerifier, state, apiKey);
            var timestamp = DateTime.Now.ToString("O");

            if (!_httpContextAccessor.HttpContext.Session.IsAvailable)
            {
                return BadRequest("Session unavailable");
            }

            _httpContextAccessor.HttpContext.Session.SetString("codeVerifier", codeVerifier);
            _httpContextAccessor.HttpContext.Session.SetString("redirectUri", redirectUri);
            _httpContextAccessor.HttpContext.Session.SetString("state", state);
            _httpContextAccessor.HttpContext.Session.SetString("timestamp", timestamp);
            _httpContextAccessor.HttpContext.Session.SetString("apiKey", apiKey);
            return Redirect(authorizationUrl);
        }

        [HttpGet("CodeFlowCallback")]
        public async Task<IActionResult> CodeFlowCallback(string code, string session_state, string state)
        {
            _httpContextAccessor.HttpContext.Session.TryGetValue("timestamp", out var timestamp);
            var timestampAsString = Encoding.UTF8.GetString(timestamp);
            var timestampAsDateTime = DateTime.ParseExact(timestampAsString, "O", CultureInfo.InvariantCulture);

            _httpContextAccessor.HttpContext.Session.TryGetValue("apiKey", out var apiKey);
            var apiKeyAsString = Encoding.UTF8.GetString(apiKey);

            var notProduction = !Primitives.Logic.Environment.IsProduction();

            if (timestampAsDateTime <= DateTime.Now.AddSeconds(-100))
            {
                _httpContextAccessor.HttpContext.Session.TryGetValue("redirectUri", out var redirectUri);
                var redirectUriAsString = Encoding.UTF8.GetString(redirectUri);
                return CodeFlowLogin(redirectUriAsString, notProduction ? apiKeyAsString : null);
            }

            _httpContextAccessor.HttpContext.Session.TryGetValue("state", out var sessionState);
            var stateAsString = Encoding.UTF8.GetString(sessionState);

            if (state != stateAsString)
            {
                _httpContextAccessor.HttpContext.Session.TryGetValue("redirectUri", out var redirectUri);
                var redirectUriAsString = Encoding.UTF8.GetString(redirectUri);
                return CodeFlowLogin(redirectUriAsString, notProduction ? apiKeyAsString : null);
            }

            _httpContextAccessor.HttpContext.Session.TryGetValue("codeVerifier", out var codeVerifier);
            var codeVerifierAsString = Encoding.UTF8.GetString(codeVerifier);

            // This function is not called directly by the frontend-applications
            // So the apiKey does not exist in a header, and we have to send it in as a parameter manually
            var tokenResponseFromIdp =
                await _oauth2Service.RequestAccessTokenCode(code, codeVerifierAsString, apiKeyAsString);

            var userReadFromToken = _jwtService.ReadAccessToken(tokenResponseFromIdp.Access_Token);

            var newJwtTokenPayload = await _jwtService.ExchangeExternalToken(userReadFromToken);

            // This function is not called directly by the frontend-applications
            // So the apiKey does not exist in a header, and we have to send it in as a parameter manually
            _cookieService.SetAccessCookieInResponse(newJwtTokenPayload.Token, apiKeyAsString);

            _httpContextAccessor.HttpContext.Session.TryGetValue("redirectUri", out var redirectUriByteArray);
            var redirectUrl = Encoding.UTF8.GetString(redirectUriByteArray);

            _httpContextAccessor.HttpContext.Session.Remove("codeVerifier");
            _httpContextAccessor.HttpContext.Session.Remove("redirectUri");
            _httpContextAccessor.HttpContext.Session.Remove("state");
            _httpContextAccessor.HttpContext.Session.Remove("timestamp");
            _httpContextAccessor.HttpContext.Session.Remove("apiKey");

            return Redirect(redirectUrl);
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout(string redirectUri, string apiKey = null)
        {
            if (Primitives.Logic.Environment.IsProduction() && !string.IsNullOrEmpty(apiKey))
            {
                return BadRequest("api-key only allowed in development");
            }

            apiKey ??= _authenticationClientService.ReadApiKey();

            var trustedApiKey = _authenticationClientService.IsApiKeyTrusted(apiKey);
            if (!trustedApiKey)
            {
                return BadRequest($"api-key: {apiKey} not valid");
            }

            await _jwtService.RevokeRefreshToken();

            _cookieService.RemoveAccessCookieInResponse(apiKey);

            return Redirect(redirectUri);
        }

        private bool IsAuthenticated(IPrincipal principal)
        {
            return principal != null && principal.Identity != null && principal.Identity.IsAuthenticated;
        }
    }
}