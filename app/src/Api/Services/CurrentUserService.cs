using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Agora.Operations.Common.Constants;
using Agora.Operations.Common.Enums;
using Agora.Operations.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Agora.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public int? DatabaseUserId
        {
            get
            {
                var stringValue = _httpContextAccessor.HttpContext?.User?.FindFirstValue(JWT.Claims.DatabaseUserId);
                if (int.TryParse(stringValue, out var returnValue)) return returnValue;
                return null;
            }
        }

        public string RefreshToken => _httpContextAccessor.HttpContext?.User?.FindFirstValue(JWT.Claims.RefreshToken);

        public string Name => _httpContextAccessor.HttpContext?.User?.FindFirstValue(JWT.Claims.Name);

        public string EmployeeName =>
            _httpContextAccessor.HttpContext?.User?.FindFirstValue(JWT.Claims.EmployeeDisplayName);

        public int? CompanyId
        {
            get
            {
                var stringValue = _httpContextAccessor.HttpContext?.User?.FindFirstValue(JWT.Claims.CompanyId);
                if (int.TryParse(stringValue, out var returnValue)) return returnValue;
                return null;
            }
        }

        public AuthenticationMethod? AuthenticationMethod => ParseAuthenticationMethod();

        private AuthenticationMethod? ParseAuthenticationMethod()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirstValue(JWT.Claims.AuthenticationMethod);
            Enum.TryParse(typeof(AuthenticationMethod), claim, out var result);
            return (AuthenticationMethod?) result;
        }

        public DateTime ExpirationDate => ParseExpirationDate();

        private DateTime ParseExpirationDate()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Expiration);
            var couldParse = DateTime.TryParse(claim, out var result);
            return couldParse ? result : DateTime.MinValue;
        }

        public DateTime MainSessionExpiration => ParseSessionExpirationDate();

        private DateTime ParseSessionExpirationDate()
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirstValue(JWT.Claims.MainSessionExpiration);
            var couldParse = DateTime.TryParse(claim, out var result);
            return couldParse ? result : DateTime.MinValue;
        }

        public List<string> Roles => ParseRoles();

        private List<string> ParseRoles()
        {
            var roleClaims = _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToList() ?? new List<string>();
            return roleClaims;

        }
    }
}