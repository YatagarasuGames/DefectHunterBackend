using MediatR;
using Shared.Models;

namespace Leaderboard.API.Services.Queries.GetAllPlayerScores
{
    public class GetAllPlayerScoresQuery : IRequest<List<PlayerScore>>
    {
    }
}
