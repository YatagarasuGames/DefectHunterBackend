using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Database.Abstractions;
using Shared.Database.Entities;
using Shared.Models;

namespace Shared.Database.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RefreshTokenRepository> _logger;

        public RefreshTokenRepository(ApplicationDbContext context, ILogger<RefreshTokenRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Guid> Create(RefreshToken refresh)
        {
            try
            {
                var refreshTokenEntity = new RefreshTokenEntity
                {
                    Id = refresh.Id,
                    Token = refresh.Token,
                    ExpireTime = refresh.ExpireTime,
                    UserId = refresh.UserId
                };

                await _context.AddAsync(refreshTokenEntity);
                await _context.SaveChangesAsync();
                return refreshTokenEntity.Id;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating token for player with id {refresh.UserId}");
                throw;
            }


        }

        public async Task<Guid> Update(Guid id, string token, DateTime expireTime, Guid userId)
        {
            try
            {
                await _context.RefreshTokens
               .Where(s => s.Id == id)
               .ExecuteUpdateAsync(s => s
               .SetProperty(p => p.Id, id)
               .SetProperty(p => p.Token, token)
               .SetProperty(p => p.ExpireTime, expireTime)
               .SetProperty(p => p.UserId, userId));

                return id;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating refresh token for player with id {id}");
                throw;
            }

        }

        public async Task<List<RefreshToken>> GetAllTokens()
        {
            try
            {
                var scoresEntities = await _context.RefreshTokens
               .AsNoTracking()
               .ToListAsync();

                var scores = scoresEntities
                .Select(t => RefreshToken.Create(t.Id, t.Token, t.ExpireTime, t.UserId))
                .Where(s => s.IsSuccess == true)
                .Select(s => s.Value)
                .ToList();

                return scores;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all refresh tokens");
                throw;
            }



        }

        public async Task<RefreshToken?> GetTokenByUserId(Guid userId)
        {
            try
            {
                var tokenEntity = await _context.RefreshTokens.FirstOrDefaultAsync(s => s.UserId == userId);
                if (tokenEntity == null) return null;
                var score = RefreshToken.Create(tokenEntity.Id, tokenEntity.Token, tokenEntity.ExpireTime, tokenEntity.UserId);
                return score.IsSuccess ? score.Value : null;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting refresh token for player with id {userId}");
                throw;
            }

        }

        public async Task<Guid> DeleteByUserId(Guid userId)
        {
            try
            {
                await _context.RefreshTokens.Where(s => s.UserId == userId).ExecuteDeleteAsync();
                return userId;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting refresh token for player with id {userId}");
                throw;
            }

        }

        public async Task<Guid> DeleteById(Guid id)
        {
            try
            {
                await _context.RefreshTokens.Where(s => s.Id == id).ExecuteDeleteAsync();
                return id;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting refresh token with id {id}");
                throw;
            }

        }

        public async Task<RefreshToken?> GetTokenByToken(string token)
        {
            try
            {
                var tokenEntity = await _context.RefreshTokens.FirstOrDefaultAsync(s => s.Token == token);
                if (tokenEntity == null) return null;
                var score = RefreshToken.Create(tokenEntity.Id, tokenEntity.Token, tokenEntity.ExpireTime, tokenEntity.UserId);
                return score.IsSuccess ? score.Value : null;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting refresh token {token}");
                throw;
            }
        }
    }
}
