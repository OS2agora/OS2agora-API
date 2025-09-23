using BallerupKommune.Operations.Common.Constants;
using BallerupKommune.Operations.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BallerupKommune.Api.Services
{
    public class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static CookieOptions AccessCookieOptions => new CookieOptions
        { HttpOnly = true, IsEssential = true, SameSite = SameSiteMode.None, Secure = true };

        public CookieService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string ReadAccessCookieFromRequest(string apiKey)
        {
            var couldReadCookie = _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(
                $"{JWT.Cookie.AccessCookieName}.{apiKey}", out var result);

            return couldReadCookie ? result : null;
        }

        public void SetAccessCookieInResponse(string value, string apiKey)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(
                $"{JWT.Cookie.AccessCookieName}.{apiKey}", value,
                AccessCookieOptions);
        }

        public void RemoveAccessCookieInResponse(string apiKey)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(
                $"{JWT.Cookie.AccessCookieName}.{apiKey}");
        }
    }
}