using Agora.Models.Enums;
using Agora.Operations.ApplicationOptions;
using Agora.Operations.Common.Enums;
using Agora.Operations.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;

namespace Agora.Api.Services
{
    public class AuthenticationClientService : IAuthenticationClientService
    {
        private readonly IOptions<AuthenticationOptions> _authOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationClientService(IOptions<AuthenticationOptions> authOptions,
            IHttpContextAccessor httpContextAccessor)
        {
            _authOptions = authOptions;
            _httpContextAccessor = httpContextAccessor;
        }

        public string ReadApiKey()
        {
            return _httpContextAccessor.HttpContext.Request.Headers["x-api-key"];
        }

        public bool IsApiKeyTrusted(string apiKey)
        {
            var trustedApiKey = _authOptions.Value.InternalApiKey == apiKey ||
                                _authOptions.Value.PublicApiKey == apiKey;

            return trustedApiKey;
        }

        public bool IsLoginAsAllowed(UserCapacity userCapacity, ClientTypes client)
        {
            switch (client)
            {
                case ClientTypes.Internal:
                    return _authOptions.Value.InternalAuthentication.Contains(userCapacity);
                case ClientTypes.Public:
                    return _authOptions.Value.PublicAuthentication.Contains(userCapacity);
                default:
                    return false;
            }
        }

        public string GetApiKeyFromOptions(ClientTypes client)
        {
            switch (client) { 
                case ClientTypes.Public:
                    return _authOptions.Value.PublicApiKey;
                case ClientTypes.Internal:
                    return _authOptions.Value.InternalApiKey;
                default:
                    return null;
            }
        }

        public ClientTypes GetClientTypeFromApiKey(string apiKey)
        {
            switch (apiKey)
            {
                case var _ when apiKey == _authOptions.Value.PublicApiKey:
                    return ClientTypes.Public;
                case var _ when apiKey == _authOptions.Value.InternalApiKey:
                    return ClientTypes.Internal;
                default:
                    return ClientTypes.None;
            }
        }

        public AuthHandlerType GetAuthHandlerTypeFromUserCapacity(UserCapacity userCapacity)
        {
            var authHandlerTypeFromOptions = "";

            switch (userCapacity)
            {
                case UserCapacity.CITIZEN:
                    authHandlerTypeFromOptions = _authOptions.Value.CitizenAuthentication;
                    break;
                case UserCapacity.COMPANY:
                    authHandlerTypeFromOptions = _authOptions.Value.CompanyAuthentication;
                    break;
                case UserCapacity.EMPLOYEE:
                    authHandlerTypeFromOptions = _authOptions.Value.EmployeeAuthentication;
                    break;
                default:
                    authHandlerTypeFromOptions = _authOptions.Value.UnknownAuthentication;
                    break;
            }

            return GetAuthHandlerType(authHandlerTypeFromOptions);
        }

        public AuthHandlerType GetAuthHandlerTypeFromAuthMethod(AuthenticationMethod authMethod)
        {
            var authHandlerTypeFromOptions = "";

            switch (authMethod)
            {
                case AuthenticationMethod.MitIdCitizen:
                    authHandlerTypeFromOptions = _authOptions.Value.CitizenAuthentication;
                    break;
                case AuthenticationMethod.MitIdErhverv:
                    authHandlerTypeFromOptions = _authOptions.Value.CompanyAuthentication;
                    break;
                case AuthenticationMethod.AdfsEmployee:
                    authHandlerTypeFromOptions = _authOptions.Value.EmployeeAuthentication;
                    break;
                default:
                    authHandlerTypeFromOptions = _authOptions.Value.UnknownAuthentication;
                    break;
            }

            return GetAuthHandlerType(authHandlerTypeFromOptions);
        }

        

        private AuthHandlerType GetAuthHandlerType(string authHandlerTypeString)
        {
            switch (authHandlerTypeString)
            {
                case var _ when authHandlerTypeString.Equals(AuthenticationOptions.AuthenticationMethods.CodeFlow, StringComparison.InvariantCultureIgnoreCase):
                    return AuthHandlerType.CodeFlow;
                case var _ when authHandlerTypeString.Equals(AuthenticationOptions.AuthenticationMethods.NemLogin, StringComparison.InvariantCultureIgnoreCase):
                    return AuthHandlerType.NemLogin;
                case var _ when authHandlerTypeString.Equals(AuthenticationOptions.AuthenticationMethods.EntraId, StringComparison.InvariantCultureIgnoreCase):
                    return AuthHandlerType.EntraId;
                default:
                    throw new NotImplementedException(
                        $"Cannot find authentication type with name {authHandlerTypeString}");
            }
        }
    }
}