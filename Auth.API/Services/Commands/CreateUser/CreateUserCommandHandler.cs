using MediatR;
using Shared.Database.Abstractions;
using Shared.Models;

namespace Auth.API.Services.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private IUsersRepository _usersRepository;
        private readonly ILogger _logger;
        public CreateUserCommandHandler(IUsersRepository usersRepository, ILogger logger)
        {
            _usersRepository = usersRepository;
            _logger = logger;
        }
        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (await _usersRepository.GetUserByEmailAsync(request.Email) != null)
            {
                _logger.LogError($"User with email {request.Email} already exists");
                throw new InvalidOperationException($"User with email {request.Email} already exists");
            }

            var userCreationResult = User.Create(request.Id, request.Username, request.Email, request.Password);

            if(userCreationResult.IsSuccess) return await _usersRepository.Create(userCreationResult.Value);
            else
            {
                _logger.LogError($"Error on creating user with email {request.Email}: {userCreationResult.Error}");
                throw new InvalidOperationException($"Error on creating user with email {request.Email}: {userCreationResult.Error}");
            }


        }
    }
}
