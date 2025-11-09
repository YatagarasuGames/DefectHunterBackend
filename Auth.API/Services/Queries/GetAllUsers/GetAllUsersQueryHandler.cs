using MediatR;
using Shared.Database.Abstractions;
using Shared.Models;

namespace Auth.API.Services.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<User>>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ILogger _logger;

        public GetAllUsersQueryHandler(IUsersRepository usersRepository, ILogger<GetAllUsersQueryHandler> logger)
        {
            _usersRepository = usersRepository;
            _logger = logger;
        }

        public async Task<List<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Getting all users");
            var users = await _usersRepository.GetAllUsers();
            _logger.LogDebug("Retrieved {UserCount} users", users.Count);

            return users;
        }
    }
}
