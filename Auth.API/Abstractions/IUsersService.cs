using Shared.Models;

namespace Auth.API.Abstractions
{
    public interface IUsersService
    {
        Task<Guid> CreateUser(User user);
        Task<Guid> DeleteUser(Guid id);
        Task<List<User>> GetAllUsers();
        Task<User?> GetUserByIdAsync(Guid id);

        Task<User?> GetUserByEmailAsync(string email);
        Task<Guid> UpdateUser(Guid guid, string username, string email, string password);
    }
}