using System;
using BallerupKommune.Operations.Common.Enums;

namespace BallerupKommune.Operations.Common.Interfaces
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
    }
}
