using MediatR;
using Shared.Database.Abstractions;

namespace Auth.API.Services.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Guid>
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ILogger _logger;
        public UpdateUserCommandHandler(IUsersRepository usersRepository, ILogger logger)
        {
            _usersRepository = usersRepository;
            _logger = logger;
        }

        public async Task<Guid> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _usersRepository.GetUserByIdAsync(request.Id);
            if (existingUser == null)
            {
                _logger.LogError($"User with id {request.Id} not found");
                throw new ArgumentException($"User with id {request.Id} not found");
            }


            return await _usersRepository.Update(request.Id, request.Username, request.Email, request.Password);
        }
    }
}
