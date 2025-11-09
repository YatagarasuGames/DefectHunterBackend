using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Database.Abstractions;
using Shared.Database.Entities;
using Shared.Models;

namespace Shared.Database.Repositories
{
    public class PlayerScoreRepository : IPlayerScoreRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public PlayerScoreRepository(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Guid> Create(PlayerScore playerScore)
        {
            try
            {
                var playerScoreEntity = new PlayerScoreEntity
                {
                    UserId = playerScore.UserId,
                    Username = playerScore.Username,
                    Score = playerScore.Score
                };

                await _context.AddAsync(playerScoreEntity);
                await _context.SaveChangesAsync();
                return playerScoreEntity.UserId;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating player score for player with id {playerScore.UserId}");
                throw;
            }


        }

        public async Task<Guid> Update(Guid id, string username, uint score)
        {
            try
            {
                 await _context.PlayerScores
                .Where(s => s.UserId == id)
                .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.UserId, id)
                .SetProperty(p => p.Username, username)
                .SetProperty(p => p.Score, score));

                return id;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating player score for player with id {id}");
                throw;
            }

        }

        public async Task<Guid> UpdateScoreOnly(Guid id, uint score)
        {

            try
            {
                await _context.PlayerScores.Where(s => s.UserId == id).ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Score, score));
                await _context.SaveChangesAsync();
                return id;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating player score for player with id {id}");
                throw;
            }


        }

        public async Task<List<PlayerScore>> GetAllScores()
        {
            try
            {
                var scoresEntities = await _context.PlayerScores
               .AsNoTracking()
               .ToListAsync();

                var scores = scoresEntities
                .Select(s => PlayerScore.Create(s.UserId, s.Username, s.Score))
                .Where(s => s.IsSuccess == true)
                .Select(s => s.Value)
                .OrderByDescending(s => s.Score)
                .ToList();

                return scores;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all player scores");
                throw;
            }


            
        }

        public async Task<PlayerScore?> GetScoreByUserId(Guid id)
        {
            try
            {
                var scoreEntity = await _context.PlayerScores.FirstOrDefaultAsync(s => s.UserId == id);
                if (scoreEntity == null) return null;
                var score = PlayerScore.Create(scoreEntity.UserId, scoreEntity.Username, scoreEntity.Score);
                return score.IsSuccess ? score.Value : null;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting player score for player with id {id}");
                throw;
            }
            
        }

        public async Task<Guid> Delete(Guid id)
        {
            try
            {
                await _context.PlayerScores.Where(s => s.UserId == id).ExecuteDeleteAsync();
                return id;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting player score for player with id {id}");
                throw;
            }
            
        }
    }
}
