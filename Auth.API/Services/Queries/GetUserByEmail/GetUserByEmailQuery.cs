using MediatR;
using Shared.Models;

namespace Auth.API.Services.Queries.GetUserByEmail
{
    public class GetUserByIdQuery : IRequest<User?>
    {
        public string Email { get; }
    }
}
