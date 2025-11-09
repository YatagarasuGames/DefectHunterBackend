using MediatR;
using Shared.Database.Abstractions;
using Shared.Models;

namespace Auth.API.Services.Queries.GetUserByEmail
{
    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, User?>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ILogger _logger;

        public GetUserByEmailQueryHandler(IUsersRepository usersRepository, ILogger logger)
        {
            _usersRepository = usersRepository;
            _logger = logger;
        }

        public async Task<User?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Getting user by email: {Email}", request.Email);

            var user = await _usersRepository.GetUserByEmailAsync(request.Email);

            if (user == null) _logger.LogDebug("User with email {Email} not found", request.Email);
            else _logger.LogDebug("User found with email {Email}, ID: {UserId}", request.Email, user.Id);
            
            return user;
        }
    }
}
