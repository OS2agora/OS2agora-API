using System;
using System.Collections.Generic;
using Agora.Operations.Common.Enums;

namespace Agora.Operations.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        int? DatabaseUserId { get; }
        AuthenticationMethod? AuthenticationMethod { get; }
        string RefreshToken { get; }
        string Name { get; }
        string EmployeeName { get; }
        int? CompanyId { get; }
        DateTime ExpirationDate { get; }
        DateTime MainSessionExpiration { get; }
        List<string> Roles {get; }
    }
}
