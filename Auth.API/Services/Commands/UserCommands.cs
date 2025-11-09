using Auth.API.Abstractions;
using Shared.Database.Abstractions;
using Shared.Models;

namespace Auth.API.Services.Commands
{
    public class UserCommands : IUserCommands
    {
        private IUsersRepository _usersRepository;
        private readonly ILogger _logger;

        public UserCommands(IUsersRepository usersRepository, ILogger logger)
        {
            _usersRepository = usersRepository;
            _logger = logger;
        }

        public async Task<Guid> CreateUser(User user)
        {
            if (await _usersRepository.GetUserByEmailAsync(user.Email) != null)
            {
                _logger.LogError($"User with email {user.Email} already exists");
                throw new InvalidOperationException($"User with email {user.Email} already exists");
            }
            return await _usersRepository.Create(user);
        }

        public async Task<Guid> UpdateUser(Guid id, string username, string email, string password)
        {
            var existingUser = await _usersRepository.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                _logger.LogError($"User with id {id} not found");
                throw new ArgumentException($"User with id {id} not found");
            }


            return await _usersRepository.Update(id, username, email, password);
        }

        public async Task<Guid> DeleteUser(Guid id)
        {
            var user = await _usersRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogError($"User with id {id} not found");
                throw new ArgumentException($"User with id {id} not found");
            }

            return await _usersRepository.Delete(id);
        }
    }
}
