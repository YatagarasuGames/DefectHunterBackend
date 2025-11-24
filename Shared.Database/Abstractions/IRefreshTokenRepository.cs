using Shared.Models;

namespace Shared.Database.Abstractions
{
    public interface IRefreshTokenRepository
    {
        Task<Guid> Create(RefreshToken refresh);
        Task<Guid> DeleteById(Guid id);
        Task<Guid> DeleteByUserId(Guid userId);
        Task<List<RefreshToken>> GetAllTokens();
        Task<RefreshToken?> GetTokenByUserId(Guid userId);
        Task<RefreshToken?> GetTokenByToken(string token);
        Task<Guid> Update(Guid id, string token, DateTime expireTime, Guid userId);
    }
}