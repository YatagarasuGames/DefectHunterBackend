using MediatR;
using Shared.Models;

namespace Auth.API.Services.Queries.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<List<User>>
    {

    }
}
