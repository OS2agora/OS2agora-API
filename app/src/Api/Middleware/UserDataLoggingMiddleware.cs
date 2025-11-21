using Agora.Operations.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System.Threading.Tasks;

namespace Agora.Api.Middleware;

public class UserDataLoggingMiddleware
{
    private readonly RequestDelegate _next;

    private const string UserIdentifierPropertyName = "UserIdentifier";
    private const string AnonymousUserIdentifier = "ANON";

    public UserDataLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
    {
        var userIdentifier = currentUserService.UserId;

        if (string.IsNullOrEmpty(userIdentifier))
        {
            userIdentifier = AnonymousUserIdentifier;
        }

        using (LogContext.PushProperty(UserIdentifierPropertyName, userIdentifier))
        {
            await _next.Invoke(context);
        }
    }
}