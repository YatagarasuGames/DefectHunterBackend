using MediatR;
using Shared.Models;

namespace Auth.API.Services.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<Guid>
    {
        public Guid Id { get; }
        public string Username { get; }
        public string Email { get; }
        public string Password { get; }
    }
}
