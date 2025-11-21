using Agora.DAOs.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Agora.DAOs.Identity
{
    public interface IRefreshTokenDao
    {
        Task<RefreshToken> Get(string refreshToken);
        Task<RefreshToken> Get(string providedRefreshToken, string applicationUserId);
        Task Invalidate(RefreshToken refreshToken);
        Task<RefreshToken> GenerateNew(string userId, string expirationTime);
    }

    // This DAO follows a different pattern than other DAOs in the project because refresh tokens are not part of the core business logic
    public class RefreshTokenDao : IRefreshTokenDao
    {
        private readonly IApplicationDbContext _db;

        private static DbContext DbAsDbContext(IApplicationDbContext db) => db as DbContext;

        public RefreshTokenDao(IApplicationDbContext db)
        {
            _db = db;
        }

        public async Task Invalidate(RefreshToken token)
        {
            token.IsRevoked = true;
            await Update(token);
        }

        public async Task<RefreshToken> GenerateNew(string userId, string expirationTime)
        {
            var expirationDate = DateTime.Now.AddSeconds(long.Parse(expirationTime)).ToLocalTime();

            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var token = Convert.ToBase64String(randomNumber);

            return await Create(new RefreshToken
            {
                ApplicationUserId = userId,
                IsRevoked = false,
                ExpirationDate = expirationDate,
                Token = token
            });
        }

        public async Task<RefreshToken> Get(string token)
        {
            var refreshTokens = await GetAll(refreshToken => refreshToken.Token == token);
            return refreshTokens.SingleOrDefault();
        }

        public async Task<RefreshToken> Get(string providedRefreshToken, string applicationUserId)
        {
            var refreshTokens = await GetAll(dbRefreshToken =>
                dbRefreshToken.Token == providedRefreshToken && dbRefreshToken.ApplicationUserId == applicationUserId);
            return refreshTokens.SingleOrDefault();
        }

        private async Task<List<RefreshToken>> GetAll(Expression<Func<RefreshToken, bool>> filter)
        {
            var query = _db.RefreshTokens.Where(filter);
            return await query.ToListAsync();
        }

        private async Task<RefreshToken> Update(RefreshToken refreshToken)
        {
            _db.RefreshTokens.Update(refreshToken);
            await DbAsDbContext(_db).SaveChangesAsync();

            return refreshToken;
        }

        private async Task<RefreshToken> Create(RefreshToken refreshToken)
        {
            await _db.RefreshTokens.AddAsync(refreshToken);
            await DbAsDbContext(_db).SaveChangesAsync();

            return refreshToken;
        } 
    }
}