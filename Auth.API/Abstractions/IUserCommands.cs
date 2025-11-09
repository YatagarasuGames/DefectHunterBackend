using Shared.Models;

namespace Auth.API.Abstractions
{
    public interface IUserCommands
    {
        Task<Guid> CreateUser(User user);
        Task<Guid> DeleteUser(Guid id);
        Task<Guid> UpdateUser(Guid id, string username, string email, string password);
    }
}