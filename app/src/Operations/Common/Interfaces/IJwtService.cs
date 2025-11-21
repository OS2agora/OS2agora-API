using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Agora.Operations.Authentication;

namespace Agora.Operations.Common.Interfaces
{
    public interface IJwtService
    {
        Task<JwtTokenPayload> LoginAndGenerateAccessToken(TokenUser tokenUser);
        ClaimsPrincipal ReadAccessToken(string accessToken);
        Task<string> RenewAccessToken(string accessToken, string refreshToken);
        Task RevokeRefreshToken();
        Task<DateTime> GetRefreshTokenExpirationDate(string refreshToken);
    }
}
