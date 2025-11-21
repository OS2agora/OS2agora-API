using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Agora.Api.Services.Interfaces;
using Agora.Operations.Authentication;
using Agora.Operations.Common.Enums;
using Agora.Operations.Common.Telemetry;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserCapacity = Agora.Models.Enums.UserCapacity;

namespace Agora.Api.Services
{
    public abstract class BaseAuthenticationHandler : IAuthenticationHandler
    {
        public abstract AuthHandlerType Type { get; }

        public abstract IActionResult HandleLogin(IHttpContextAccessor httpContextAccessor, string redirectUri, string loginAs,
            string apiKey, UserCapacity userCapacity);

        public abstract Task<IActionResult> HandleLogout(IHttpContextAccessor httpContextAccessor, string redirectUri,
            string apiKey);

        public abstract TokenUser ParseUserFromClaims(ClaimsPrincipal token);

        public virtual Task<IActionResult> HandleOAuthCallback(IHttpContextAccessor httpContextAccessor, string code, string state, Func<string, string, string, IActionResult> doLogin)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IActionResult> HandleEntraIdCallback(IHttpContextAccessor httpContextAccessor, string code, string state, Func<string, string, string, IActionResult> doLogin)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IActionResult> HandleNemLoginCallback(IHttpContextAccessor httpContextAccessor, string defaultRedirectUrl)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IActionResult> HandleNemLoginSingleLogout(IHttpContextAccessor httpContextAccessor,
            string defaultRedirectUrl, string apiKey = null)
        {
            throw new NotImplementedException();
        }

        public virtual IActionResult HandleNemLoginLogoutResponse(IHttpContextAccessor httpContextAccessor,
            string defaultRedirectUrl)
        {
            throw new NotImplementedException();
        }

        protected static Activity AuthenticationHandlerActivity(string authenticationHandler, [CallerMemberName] string methodName = "")
        {
            var activity = Instrumentation.Source.StartActivity($"AuthenticationHandler: {authenticationHandler} - {methodName}");
            return activity;
        }
    }
}