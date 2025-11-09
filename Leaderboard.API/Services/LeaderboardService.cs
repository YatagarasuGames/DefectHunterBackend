using Leaderboard.API.Abstractions;
using Shared.Database.Abstractions;
using Shared.Models;

namespace Leaderboard.API.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private IPlayerScoreRepository _playerScoreRepository;

        public LeaderboardService(IPlayerScoreRepository playerScoreRepository)
        {
            _playerScoreRepository = playerScoreRepository;
        }

        public async Task<Guid> CreatePlayerScore(PlayerScore playerScore)
        {
            return await _playerScoreRepository.Create(playerScore);
        }

        public async Task<List<PlayerScore>> GetAllPlayerScores()
        {
            return await _playerScoreRepository.GetAllScores();
        }

        public async Task<PlayerScore?> GetPlayerScoreByUserId(Guid id)
        {
            return await _playerScoreRepository.GetScoreByUserId(id);
        }

        public async Task<Guid> UpdatePlayerScore(Guid id, string username, uint score)
        {
            return await _playerScoreRepository.Update(id, username, score);
        }

        public async Task<Guid> UpdateOnlyPlayerScore(Guid id, uint score)
        {
            return await _playerScoreRepository.UpdateScoreOnly(id, score);
        }

        public async Task<Guid> Delete(Guid id)
        {
            return await _playerScoreRepository.Delete(id);
        }
    }
}
