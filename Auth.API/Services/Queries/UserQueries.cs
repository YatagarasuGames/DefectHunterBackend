using Auth.API.Abstractions;
using Shared.Database.Abstractions;
using Shared.Models;

namespace Auth.API.Services.Queries
{
    public class UserQueries : IUserQueries
    {
        private readonly IUsersRepository _usersRepository;

        public UserQueries(IUsersRepository usersRepository)
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
    }
}
