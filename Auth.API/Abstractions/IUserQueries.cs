using Shared.Models;

namespace Auth.API.Services.Queries
{
    public interface IUserQueries
    {
        Task<List<User>> GetAllUsers();
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(Guid id);
    }
}