using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Shared.Database.Abstractions
{
    public interface IPlayerScoreRepository
    {
        Task<Guid> Create(PlayerScore playerScore);
        Task<Guid> Delete(Guid id);
        Task<List<PlayerScore>> GetAllScores();
        Task<PlayerScore?> GetScoreByUserId(Guid id);
        Task<Guid> Update(Guid id, string username, uint score);
        Task<Guid> SetScore(Guid id, uint score);
        Task<Guid> AddScore(Guid id, uint score);
    }
}