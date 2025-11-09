using MediatR;

namespace Auth.API.Services.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest<Guid>
    {
        public Guid Id { get; }
    }
}
