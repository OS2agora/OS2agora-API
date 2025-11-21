using Agora.Api.Services.Interfaces;
using Agora.Operations.Common.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Agora.Api.Services.AuthenticationHandlers
{
    public class AuthenticationHandlerFactory : IAuthenticationHandlerFactory
    {
        private readonly IList<IAuthenticationHandler> _handlerRegistry;

        public AuthenticationHandlerFactory(IEnumerable<IAuthenticationHandler> handlers)
        {
            _handlerRegistry = handlers.ToList();
        }

        public IAuthenticationHandler CreateAuthenticationHandler(AuthHandlerType type)
        {
            return _handlerRegistry.Single(handler => handler.Type == type);
        }
    }
}