using Agora.Operations.Common.Enums;

namespace Agora.Api.Services.Interfaces
{
    public interface IAuthenticationHandlerFactory
    {
        IAuthenticationHandler CreateAuthenticationHandler(AuthHandlerType type);
    }
}