using System.Security.Claims;
using System.Threading.Tasks;
using BallerupKommune.Operations.Authentication;

namespace BallerupKommune.Operations.Common.Interfaces
{
    public interface IJwtService
    {
        Task<JwtTokenPayload> ExchangeExternalToken(ClaimsPrincipal userReadFromToken);
        ClaimsPrincipal ReadAccessToken(string accessToken);
        Task<string> RenewAccessToken(string accessToken, string refreshToken);
        Task RevokeRefreshToken();
    }
}