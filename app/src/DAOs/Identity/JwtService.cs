using BallerupKommune.Models.Models;
using BallerupKommune.Operations.ApplicationOptions;
using BallerupKommune.Operations.Authentication;
using BallerupKommune.Operations.Common.Constants;
using BallerupKommune.Operations.Common.Enums;
using BallerupKommune.Operations.Common.Interfaces;
using BallerupKommune.Operations.Models.Users.Command.LoginUser;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Identity
{
    public class JwtService : IJwtService
    {
        private readonly IOptions<JwtSettingsOptions> _jwtSettingsOptions;
        private readonly IIdentityService _identityService;
        private readonly ISender _mediator;
        private readonly IRefreshTokenDao _refreshTokenDao;
        private readonly ICurrentUserService _currentUserService;

        public JwtService(IOptions<JwtSettingsOptions> jwtSettingsOptions, IIdentityService identityService,
            ISender mediator, IRefreshTokenDao refreshTokenDao, ICurrentUserService currentUserService)
        {
            _jwtSettingsOptions = jwtSettingsOptions;
            _identityService = identityService;
            _mediator = mediator;
            _refreshTokenDao = refreshTokenDao;
            _currentUserService = currentUserService;
        }

        public async Task<string> RenewAccessToken(string accessToken, string refreshToken)
        {
            // Find refreshToken
            var token = await _refreshTokenDao.Get(refreshToken);

            // Validate refresh token (isExpired, isRevoked etc.)
            var isTokenValid = ValidateRefreshToken(token, _currentUserService.UserId, _currentUserService.RefreshToken);

            if (!isTokenValid)
            {
                throw new Exception("Refresh token was not valid");
            }

            // Invalidate old refresh token
            await _refreshTokenDao.Invalidate(token);

            // Generate new refresh Token
            var refreshTokenExpiration = _jwtSettingsOptions.Value.SecondsToRefreshTokenExpiration;
            var newRefreshToken = await _refreshTokenDao.GenerateNew(_currentUserService.UserId, refreshTokenExpiration);

            var tokenUser = new TokenUser
            {
                Name = _currentUserService.Name,
                EmployeeDisplayName = _currentUserService.EmployeeName,
                ApplicationUserId = _currentUserService.UserId,
                AuthMethod = (AuthenticationMethod)_currentUserService?.AuthenticationMethod
            };

            // Generate new access Token
            var expiration = _jwtSettingsOptions.Value.SecondsToAccessTokenExpiration;
            var accessTokenDescriptor = CreateSecurityTokenDescriptor(
                tokenUser,
                expiration, 
                newRefreshToken.Token);

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var createToken = jwtSecurityTokenHandler.CreateToken(accessTokenDescriptor);
            var newAccessToken = jwtSecurityTokenHandler.WriteToken(createToken);

            return newAccessToken;
        }

        public async Task RevokeRefreshToken()
        {
            var refreshTokenFromClaim = _currentUserService.RefreshToken;

            var refreshToken = await _refreshTokenDao.Get(refreshTokenFromClaim);

            if (refreshToken != null)
            {
                await _refreshTokenDao.Invalidate(refreshToken);
            }
        }

        public async Task<JwtTokenPayload> ExchangeExternalToken(ClaimsPrincipal userReadFromToken)
        {
            var tokenUser = await GetTokenUser(userReadFromToken);

            var expiration = _jwtSettingsOptions.Value.SecondsToAccessTokenExpiration;

            // Generate new refresh Token
            var refreshTokenExpiration = _jwtSettingsOptions.Value.SecondsToRefreshTokenExpiration;
            var refreshToken = await _refreshTokenDao.GenerateNew(tokenUser.ApplicationUserId, refreshTokenExpiration);

            // Create token descriptor to hold the JWT payload
            var securityTokenDescriptor = CreateSecurityTokenDescriptor(tokenUser, expiration, refreshToken.Token);

            // Add roles from external IdP to the application
            if (tokenUser.PossibleRoles.Any())
            {
                if (tokenUser.PossibleRoles.Any(r => r.Value == ExternalIdP.Groups.Admin))
                {
                    await _identityService.AddUserToRole(tokenUser.ApplicationUserId, JWT.Roles.Admin);
                    securityTokenDescriptor.Subject.AddClaim(new Claim(type: ClaimTypes.Role, JWT.Roles.Admin));
                }

                if (tokenUser.PossibleRoles.Any(r => r.Value == ExternalIdP.Groups.CanCreateHearing))
                {
                    await _identityService.AddUserToRole(tokenUser.ApplicationUserId, JWT.Roles.HearingCreator);
                    securityTokenDescriptor.Subject.AddClaim(new Claim(type: ClaimTypes.Role,
                        JWT.Roles.HearingCreator));
                }
            }
            
            // Populate the return object with some extra goodies for ease of use
            var isAdministrator = securityTokenDescriptor.Subject.HasClaim(ClaimTypes.Role, JWT.Roles.Admin);
            var isHearingOwner = securityTokenDescriptor.Subject.HasClaim(ClaimTypes.Role, JWT.Roles.HearingCreator);

            // Login user into application
            var user = await LoginUser(tokenUser, isAdministrator, isHearingOwner);

            // Add database user id to claim for security expressions
            securityTokenDescriptor.Subject.AddClaim(new Claim(type: JWT.Claims.DatabaseUserId, user.Id.ToString()));
            securityTokenDescriptor.Subject.AddClaim(new Claim(type: JWT.Claims.CompanyId, user.CompanyId.ToString()));

            // Generate the JWT token
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var createToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            var resultToken = jwtSecurityTokenHandler.WriteToken(createToken);

            // Note: This might not be necessary since all the information should be in the token...
            // Note: The frontend doesn't actually use the tokens content, it just uses it for authentication.
            return new JwtTokenPayload
            {
                Token = resultToken,
                UserId = user.Id,
                ApplicationUserId = tokenUser.ApplicationUserId,
                Name = tokenUser.Name,
                EmployeeDisplayName = tokenUser.EmployeeDisplayName,
                IsAdministrator = isAdministrator,
                IsHearingOwner = isHearingOwner,
                AuthenticationMethod = tokenUser.AuthMethod.ToString()
            };
        }

        private async Task<TokenUser> GetTokenUser(ClaimsPrincipal token)
        {
            var tokenUser = new TokenUser
            {
                Cpr = token.FindFirstValue(ExternalIdP.Claims.CprNumberIdentifier),
                Cvr = token.FindFirstValue(ExternalIdP.Claims.CvrNumberIdentifier),
                Email = token.FindFirstValue(ExternalIdP.Claims.Email),
                Pid = token.FindFirstValue(ExternalIdP.Claims.PidNumberIdentifier),
                PossibleRoles = token.FindAll(ExternalIdP.Claims.Roles).ToList(),
                CompanyName = token.FindFirstValue(ExternalIdP.Claims.CompanyNameIdentifier)
            };

            SetAuthenticationMethod(tokenUser);
            SetEmployeeDisplayName(tokenUser, token);
            SetPersonalIdentifier(tokenUser);
            SetName(tokenUser, token);

            tokenUser.ApplicationUserId = await _identityService.FindUserOrCreateUser(tokenUser.PersonalIdentifier);

            return tokenUser;
        }


        private void SetAuthenticationMethod(TokenUser user)
        {
            if (user.Cvr != null)
            {
                user.AuthMethod = AuthenticationMethod.MitIdErhverv;
            }
            else if (user.Pid == null)
            {
                user.AuthMethod = AuthenticationMethod.AdfsEmployee;
            }
            else
            {
                user.AuthMethod = AuthenticationMethod.MitIdCitizen;
            }
        }

        private void SetEmployeeDisplayName(TokenUser user, ClaimsPrincipal token)
        {

            if (user.AuthMethod == AuthenticationMethod.AdfsEmployee)
            {
                user.EmployeeDisplayName = token.FindFirstValue(ExternalIdP.Claims.DisplayName);
            }
        }

        private void SetPersonalIdentifier(TokenUser user)
        {
            switch (user.AuthMethod)
            {
                case AuthenticationMethod.MitIdErhverv:
                    user.PersonalIdentifier = $"{user.Cvr}-{user.Cpr}";
                    return;
                case AuthenticationMethod.MitIdCitizen:
                    user.PersonalIdentifier = user.Cpr;
                    return;
                case AuthenticationMethod.AdfsEmployee:
                    user.PersonalIdentifier = user.Email;
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(user.AuthMethod), user.AuthMethod, null);
            }
        }

        private void SetName(TokenUser user, ClaimsPrincipal token)
        {
            if (user.AuthMethod == AuthenticationMethod.AdfsEmployee)
            {
                var firstName = token.FindFirstValue(ExternalIdP.Claims.FirstName);
                var lastName = token.FindFirstValue(ExternalIdP.Claims.LastName);
                user.Name = $"{firstName} {lastName}";
            }
            else
            {
                user.Name = token.FindFirstValue(ExternalIdP.Claims.FullName);
            }
        }

        public ClaimsPrincipal ReadAccessToken(string accessToken)
        {
            var jwtSecurityToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var claims = jwtSecurityToken.Claims;
            return new ClaimsPrincipal(new ClaimsIdentity(claims));
        }

        private SecurityTokenDescriptor CreateSecurityTokenDescriptor(TokenUser tokenUser,
            string expiresIn, string refreshToken)
        {
            var expiration = DateTime.Now.AddSeconds(long.Parse(expiresIn)).ToLocalTime();
            var key = Encoding.ASCII.GetBytes(_jwtSettingsOptions.Value.Secret);

            // This must be signed with the same key as registered in DependencyInjection.cs
            // This is to ensure the automatic authentication pipeline can validate the token issued
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, tokenUser.ApplicationUserId),
                    new Claim(JWT.Claims.Name, tokenUser.Name),
                    new Claim(JWT.Claims.AuthenticationMethod, tokenUser.AuthMethod.ToString()), 
                    new Claim(JWT.Claims.RefreshToken, refreshToken), 
                    new Claim(ClaimTypes.Expiration, expiration.ToString()), 
                }),
                Expires = expiration,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
            };

            if (!string.IsNullOrEmpty(tokenUser.EmployeeDisplayName))
            {
                tokenDescriptor.Subject.AddClaim(new Claim(JWT.Claims.EmployeeDisplayName, tokenUser.EmployeeDisplayName));
            }

            return tokenDescriptor;
        }

        private async Task<User> LoginUser(TokenUser tokenUser, bool isAdministrator, bool isHearingCreator)
        {
            return await _mediator.Send(new LoginUserCommand
            {
                TokenUser = tokenUser,
                IsAdministrator = isAdministrator,
                IsHearingCreator = isHearingCreator
            });
        }

        private bool ValidateRefreshToken(RefreshToken token, string userId, string tokenFromClaim)
        {
            if (token.Token != tokenFromClaim)
            {
                return false;
            }

            if (token.ExpirationDate < DateTime.Now)
            {
                return false;
            }

            if (token.IsRevoked)
            {
                return false;
            }

            if (token.ApplicationUserId != userId)
            {
                return false;
            }

            return true;
        }
    }
}