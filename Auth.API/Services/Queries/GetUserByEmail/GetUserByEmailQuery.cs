using MediatR;
using Shared.Models;

namespace Auth.API.Services.Queries.GetUserByEmail
{
    public class GetUserByEmailQuery : IRequest<User?>
    {
        public string Email { get; }

        public GetUserByEmailQuery(string email)
        {
            Email = email;
        }
    }
}
