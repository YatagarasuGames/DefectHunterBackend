using MediatR;
using Shared.Models;

namespace Auth.API.Services.Queries.GetUserById
{
    public class GetUserByIdQuery : IRequest<User?>
    {
        public Guid Id { get; }
    }
}
