using MediatR;
using Shared.Models;

namespace Leaderboard.API.Services.Queries.GetPlayerScoreByUserId
{
    public class GetPlayerScoreByUserIdQuery : IRequest<PlayerScore>
    {
        public Guid Id { get; }
        public GetPlayerScoreByUserIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
