using Agora.Operations.Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Agora.Models.Enums;
using Agora.Operations.Authentication;
using Agora.Operations.Common.Interfaces;

namespace Agora.Api.Services.Interfaces
{
    public interface IAuthenticationHandler
    {
        AuthHandlerType Type { get; }

        IActionResult HandleLogin(IHttpContextAccessor httpContextAccessor, string redirectUri, string loginAs, string apiKey, UserCapacity userCapacity);
        Task<IActionResult> HandleLogout(IHttpContextAccessor httpContextAccessor, string redirectUri, string apiKey);
        Task<IActionResult> HandleOAuthCallback(IHttpContextAccessor httpContextAccessor, string code, string state, Func<string, string, string, IActionResult> doLogin);
        Task<IActionResult> HandleEntraIdCallback(IHttpContextAccessor httpContextAccessor, string code, string state, Func<string, string, string, IActionResult> doLogin);
        Task<IActionResult> HandleNemLoginCallback(IHttpContextAccessor httpContextAccessor, string defaultRedirectUrl);

        Task<IActionResult> HandleNemLoginSingleLogout(IHttpContextAccessor httpContextAccessor,
            string defaultRedirectUrl, string apiKey = null);

        IActionResult HandleNemLoginLogoutResponse(IHttpContextAccessor httpContextAccessor,
            string defaultRedirectUrl);
        TokenUser ParseUserFromClaims(ClaimsPrincipal token);
    }
}