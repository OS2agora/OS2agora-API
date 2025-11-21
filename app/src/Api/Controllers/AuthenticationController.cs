using Agora.Api.Models.JsonApi;
using Agora.Api.Services.Interfaces;
using Agora.DTOs.Models;
using Agora.Models.Models;
using Agora.Operations.Common.Enums;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Models.Users.Queries.GetMe;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UserCapacityEnum = Agora.Models.Enums.UserCapacity;
using UserDto = Agora.Api.Models.DTOs.UserDto;

namespace Agora.Api.Controllers
{
    public class AuthenticationController : ApiController
    {
        private readonly IJwtService _jwtService;
        private readonly ICookieService _cookieService;
        private readonly IOauth2Service _oauth2Service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticationClientService _authenticationClientService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthenticationHandlerFactory _authenticationHandlerFactory;

        public AuthenticationController(IJwtService jwtService, ICookieService cookieService,
            IOauth2Service oauth2Service, IHttpContextAccessor httpContextAccessor,
            IAuthenticationClientService authenticationClientService, ICurrentUserService currentUserService, IAuthenticationHandlerFactory authenticationHandlerFactory)
        {
            _cookieService = cookieService;
            _oauth2Service = oauth2Service;
            _httpContextAccessor = httpContextAccessor;
            _authenticationClientService = authenticationClientService;
            _currentUserService = currentUserService;
            _jwtService = jwtService;
            _authenticationHandlerFactory = authenticationHandlerFactory;
        }

        [HttpGet("Authorized")]
        public IActionResult Authorized()
        {
            var isAuthenticated = IsAuthenticated(_httpContextAccessor?.HttpContext?.User);
            return Ok(isAuthenticated);
        }

        [Authorize]
        [HttpGet("RefreshToken")]
        public async Task<ActionResult<RefreshTokenDto>> GetRefreshToken()
        {
            var refreshToken = _currentUserService.RefreshToken;
            var accessTokenExpiration = _currentUserService.ExpirationDate;

            if (refreshToken == null || accessTokenExpiration == default)
            {
                return Forbid();
            }

            var urlEncodedToken = HttpUtility.UrlEncode(refreshToken, Encoding.ASCII);
            var refreshTokenExpirationDate = await _jwtService.GetRefreshTokenExpirationDate(refreshToken);

            var result = new RefreshTokenDto
            {
                Token = urlEncodedToken,
                AccessTokenExpirationDate = accessTokenExpiration,
                RefreshTokenExpirationDate = refreshTokenExpirationDate
            };

            return Ok(result);
        }

        [HttpPost("RenewAccessToken")]
        public async Task<ActionResult<RefreshTokenDto>> RenewAccessToken([FromQuery] string refreshToken)
        {
            var apiKey = _authenticationClientService.ReadApiKey();
            var currentAccessToken = _cookieService.ReadAccessCookieFromRequest(apiKey);

            string newAccessToken;
            try
            {
                // Update access token
                newAccessToken = await _jwtService.RenewAccessToken(currentAccessToken, refreshToken);
            }
            catch (Exception)
            {
                // If an exception is thrown, the token cannot be renewed and the session should be terminated. 
                await _jwtService.RevokeRefreshToken();
                _cookieService.RemoveAccessCookieInResponse(apiKey);
                return Forbid();
            }

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

                var authHandler = _authenticationHandlerFactory
                    .CreateAuthenticationHandler(AuthHandlerType.CodeFlow);

                var tokenUser = authHandler.ParseUserFromClaims(userReadFromToken);

                var newJwtTokenPayload = await _jwtService.LoginAndGenerateAccessToken(tokenUser);

                _cookieService.SetAccessCookieInResponse(newJwtTokenPayload.Token, apiKey);
                return Ok(newJwtTokenPayload.Token);

            } catch (Exception e)
            {
                var AuthURL = new Uri(_oauth2Service.GetAuthenticationEndpoint());
                return BadRequest($"Could not find user with the provided username. Find all allowed users in the dropdown here: {AuthURL.Scheme}://localhost:{AuthURL.Port}");
            }
        }
        
        [HttpGet("Login")]
        public IActionResult Login(string redirectUri, string loginAs, string apiKey = null)
        {
            var apiKeyValidationResult = ValidateApiKey(apiKey, out var clientType, out var validatedApiKey);
            if (apiKeyValidationResult != null)
            {
                return apiKeyValidationResult;
            }

            var userCapacityValidationResult = ValidateUserCapacity(loginAs, clientType, out var userCapacity);
            if (userCapacityValidationResult != null)
            {
                return userCapacityValidationResult;
            }

            var authHandlerType = _authenticationClientService.GetAuthHandlerTypeFromUserCapacity(userCapacity);
            var authHandler = _authenticationHandlerFactory.CreateAuthenticationHandler(authHandlerType);
            return authHandler.HandleLogin(_httpContextAccessor, redirectUri, loginAs, validatedApiKey, userCapacity);
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout(string redirectUri, string apiKey = null)
        {
            var validationResult = ValidateApiKey(apiKey, out var clientType, out var validatedApiKey);
            if (validationResult != null)
            {
                return validationResult;
            }

            var authMethod = _currentUserService.AuthenticationMethod;
            if (authMethod == null)
            {
                await _jwtService.RevokeRefreshToken();
                _cookieService.RemoveAccessCookieInResponse(apiKey);
                return Redirect(redirectUri);
            }

            var authHandlerType = _authenticationClientService.GetAuthHandlerTypeFromAuthMethod(authMethod.Value);
            var authHandler = _authenticationHandlerFactory.CreateAuthenticationHandler(authHandlerType);
            return await authHandler.HandleLogout(_httpContextAccessor, redirectUri, validatedApiKey);

        }

        [HttpGet("CodeFlowCallback")]
        public async Task<IActionResult> CodeFlowCallback(string code, string state)
        {
            var authHandler = _authenticationHandlerFactory.CreateAuthenticationHandler(AuthHandlerType.CodeFlow);
            return await authHandler.HandleOAuthCallback(_httpContextAccessor, code, state, Login);
        }

        [HttpGet("EntraIdCallback")]
        public async Task<IActionResult> EntraIdCallback(string code, string state)
        {
            var authHandler = _authenticationHandlerFactory.CreateAuthenticationHandler(AuthHandlerType.EntraId);
            return await authHandler.HandleEntraIdCallback(_httpContextAccessor, code, state, Login);
        }


        [Route("saml2/assertionconsumerservice")]
        public async Task<IActionResult> AssertionConsumerService()
        {
            var defaultRedirectUri = Url.Content("~/");
            var authHandler = _authenticationHandlerFactory.CreateAuthenticationHandler(AuthHandlerType.NemLogin);
            return await authHandler.HandleNemLoginCallback(_httpContextAccessor, defaultRedirectUri);
        }

        [Route("saml2/singlelogout")]
        public async Task<IActionResult>SingleLogout()
        {
            var defaultRedirectUri = Url.Content("~/");
            var apiKey = _authenticationClientService.ReadApiKey();
            var authHandler = _authenticationHandlerFactory.CreateAuthenticationHandler(AuthHandlerType.NemLogin);
            return await authHandler.HandleNemLoginSingleLogout(_httpContextAccessor, defaultRedirectUri, apiKey);
        }

        [Route("saml2/loggedout")]
        public IActionResult LoggedOut()
        {
            var defaultRedirectUri = Url.Content("~/");
            var authHandler = _authenticationHandlerFactory.CreateAuthenticationHandler(AuthHandlerType.NemLogin);
            return authHandler.HandleNemLoginLogoutResponse(_httpContextAccessor, defaultRedirectUri);
        }

        private IActionResult ValidateUserCapacity(string loginAs, ClientTypes client, out UserCapacityEnum userCapacity)
        {
            if (!Enum.TryParse<UserCapacityEnum>(loginAs, out userCapacity))
            {
                return new BadRequestObjectResult("Invalid UserCapacity");
            }

            var isLoginAllowed = _authenticationClientService.IsLoginAsAllowed(userCapacity, client);
            if (!isLoginAllowed)
            {
                return new BadRequestObjectResult(
                    $"Cannot login as {nameof(userCapacity)} on client of type {nameof(client)}");
            }

            return null;
        }

        private IActionResult ValidateApiKey(string apiKey, out ClientTypes clientType, out string validatedApiKey)
        {
            clientType = ClientTypes.None;
            validatedApiKey = "";

            // Check API key
            if (Primitives.Logic.Environment.IsProduction() && !string.IsNullOrEmpty(apiKey))
            {
                return new BadRequestObjectResult("api-key only allowed in development");
            }

            apiKey ??= _authenticationClientService.ReadApiKey();

            var trustedApiKey = _authenticationClientService.IsApiKeyTrusted(apiKey);
            if (!trustedApiKey)
            {
                return new BadRequestObjectResult($"api-key: {apiKey} not valid");
            }

            clientType = _authenticationClientService.GetClientTypeFromApiKey(apiKey);
            validatedApiKey = apiKey;

            return null;
        }

        private bool IsAuthenticated(IPrincipal principal)
        {
            return principal != null && principal.Identity != null && principal.Identity.IsAuthenticated;
        }
    }
}