using Auth.API.Abstractions;
using Shared.Database.Abstractions;
using Shared.Models;

namespace Auth.API.Services
{
    public class UsersService : IUsersService
    {
        private IUsersRepository _usersRepository;

        public UsersService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _usersRepository.GetAllUsers();
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _usersRepository.GetUserByIdAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _usersRepository.GetUserByEmailAsync(email);
        }

        public async Task<Guid> CreateUser(User user)
        {
            return await _usersRepository.Create(user);
        }

        public async Task<Guid> UpdateUser(Guid guid, string username, string email, string password)
        {
            return await _usersRepository.Update(guid, username, email, password);
        }

        public async Task<Guid> DeleteUser(Guid id)
        {
            return await _usersRepository.Delete(id);
        }
    }
}
