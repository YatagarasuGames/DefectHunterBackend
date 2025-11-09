using MediatR;
using Shared.Database.Abstractions;
using Shared.Models;

namespace Auth.API.Services.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, User?>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ILogger<GetUserByIdQueryHandler> _logger;

        public GetUserByIdQueryHandler(IUsersRepository usersRepository, ILogger<GetUserByIdQueryHandler> logger)
        {
            _usersRepository = usersRepository;
            _logger = logger;
        }

        public async Task<User?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Getting user by email: {Email}", request.Id);

            var user = await _usersRepository.GetUserByIdAsync(request.Id);

            if (user == null) _logger.LogDebug("User with Id {Id} not found", request.Id);
            else _logger.LogDebug("User found with Id {Id}", request.Id);
            
            return user;
        }
    }
}
