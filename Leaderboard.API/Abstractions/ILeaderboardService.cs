using Shared.Models;

namespace Leaderboard.API.Abstractions
{
    public interface ILeaderboardService
    {
        Task<Guid> CreatePlayerScore(PlayerScore playerScore);
        Task<Guid> Delete(Guid id);
        Task<List<PlayerScore>> GetAllPlayerScores();
        Task<Guid> UpdatePlayerScore(Guid id, string username, uint score);
        Task<Guid> UpdateOnlyPlayerScore(Guid id, uint score);
        Task<PlayerScore?> GetPlayerScoreByUserId(Guid id);
    }
}