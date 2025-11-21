using Microsoft.AspNetCore.Http;
using System;
using Agora.Operations.Common.Exceptions;

namespace Agora.Api.Services.AuthenticationHandlers.utility
{
    public static class SessionKeys
    {
        public const string CodeVerifier = "codeVerifier";
        public const string RedirectUri = "redirectUri";
        public const string Timestamp = "timestamp";
        public const string ApiKey = "apiKey";
        public const string State = "state";
        public const string LoginAs = "loginAs";
        public const string UserCapacity = "userCapacity";
    }

    public static class SessionExtensions
    {
        public static string GetSessionValue(this IHttpContextAccessor httpContextAccessor, string key, bool allowNull = false)
        {
            var valueFound = httpContextAccessor.HttpContext.Session.TryGetValue(key, out var value);

            if (!valueFound && !allowNull)
            {
                throw new ForbiddenAccessException();
            }

            var valueAsString = System.Text.Encoding.UTF8.GetString(value);
            return valueAsString;
        }

        public static T GetSessionValue<T>(this IHttpContextAccessor httpContextAccessor, string key, Func<string, T> valueFromString, bool allowNull = false)
        {
            var valueAsString = GetSessionValue(httpContextAccessor, key);
            return valueFromString(valueAsString);
        }

        public static void SetSessionValue(this IHttpContextAccessor httpContextAccessor, string key, string value)
        {
            httpContextAccessor.HttpContext.Session.SetString(key, value);
        }

        public static void RemoveSessionValue(this IHttpContextAccessor httpContextAccessor, string key)
        {
            httpContextAccessor.HttpContext.Session.Remove(key);
        }
    }
}