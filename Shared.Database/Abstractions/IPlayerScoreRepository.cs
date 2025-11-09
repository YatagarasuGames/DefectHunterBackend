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
        Task<Guid> UpdateScoreOnly(Guid id, uint score);
    }
}