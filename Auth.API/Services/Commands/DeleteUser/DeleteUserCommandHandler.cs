using MediatR;
using Shared.Database.Abstractions;

namespace Auth.API.Services.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Guid>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ILogger<DeleteUserCommandHandler> _logger;

        public DeleteUserCommandHandler(IUsersRepository usersRepository, ILogger<DeleteUserCommandHandler> logger)
        {
            _usersRepository = usersRepository;
            _logger = logger;
        }

        public async Task<Guid> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _usersRepository.GetUserByIdAsync(request.Id);
            if (user == null)
            {
                _logger.LogError($"User with id {request.Id} not found");
                throw new ArgumentException($"User with id {request.Id} not found");
            }

            return await _usersRepository.Delete(request.Id);
        }

    }
}
