using MediatR;

namespace Auth.API.Services.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<Guid>
    {
        public Guid Id { get; }
        public string Username { get; }
        public string Email { get; }
        public string Password { get; }

        public UpdateUserCommand(Guid id, string username, string email, string password)
        {
            Id = id;
            Username = username;
            Email = email;
            Password = password;
        }
    }
}
