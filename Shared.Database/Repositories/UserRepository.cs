using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Database.Abstractions;
using Shared.Database.Entities;
using Shared.Models;

namespace Shared.Database.Repositories
{

    public class UserRepository : IUsersRepository
    {

        public readonly ApplicationDbContext _context;
        public readonly ILogger _logger;

        public UserRepository(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<User>> GetAllUsers()
        {
            try
            {
                var usersEntities = await _context.Users
                .AsNoTracking()
                .ToListAsync();

                var users = usersEntities
                    .Select(u => User.Create(u.Id, u.Username, u.Email, u.Password))
                    .Where(result => result.IsSuccess)
                    .Select(result => result.Value)
                    .ToList();

                return users;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null) return null;

                var result = User.Create(user.Id, user.Username, user.Email, user.Password);
                return result.IsSuccess ? result.Value : null;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by Id");
                throw;
            }
            
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null) return null;

                var result = User.Create(user.Id, user.Username, user.Email, user.Password);
                return result.IsSuccess ? result.Value : null;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by Email");
                throw;
            }
        }


        public async Task<Guid> Create(User user)
        {
            try
            {
                var userEntity = new UserEntity
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Password = user.Password,
                };

                await _context.AddAsync(userEntity);
                await _context.SaveChangesAsync();

                return userEntity.Id;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with email {Email}", user.Email);
                throw new InvalidOperationException("Could not create user", ex);
            }

        }

        public async Task<Guid> Update(Guid id, string username, string email, string password)
        {
            try
            {
                 await _context.Users
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(s => s
                .SetProperty(u => u.Username, username)
                .SetProperty(u => u.Email, email)
                .SetProperty(s => s.Password, password));


                return id;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user with Email {email}");
                throw;
            }

        }


        public async Task<Guid> Delete(Guid id)
        {
            try
            {
                await _context.Users
                .Where(b => b.Id == id)
                .ExecuteDeleteAsync();

                return id;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user with Email {id}");
                throw;
            }

        }
    }
}
