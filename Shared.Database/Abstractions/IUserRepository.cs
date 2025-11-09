using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Shared.Database.Abstractions
{
    public interface IUsersRepository
    {
        Task<Guid> Create(User user);
        Task<Guid> Delete(Guid id);
        Task<List<User>> GetAllUsers();
        Task<User?> GetUserByIdAsync(Guid id);

        Task<User?> GetUserByEmailAsync(string email);
        Task<Guid> Update(Guid id, string username, string email, string password);
    }
}