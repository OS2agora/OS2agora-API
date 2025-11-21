using Agora.Models.Enums;
using Agora.Operations.Common.Enums;

namespace Agora.Operations.Common.Interfaces
{
    public interface IAuthenticationClientService
    {
        public bool IsApiKeyTrusted(string apiKey);
        string ReadApiKey();
        string GetApiKeyFromOptions(ClientTypes client);
        bool IsLoginAsAllowed(UserCapacity userCapacity, ClientTypes client);
        ClientTypes GetClientTypeFromApiKey(string apiKey);
        AuthHandlerType GetAuthHandlerTypeFromUserCapacity(UserCapacity userCapacity);
        AuthHandlerType GetAuthHandlerTypeFromAuthMethod(AuthenticationMethod authMethod);
    }
}