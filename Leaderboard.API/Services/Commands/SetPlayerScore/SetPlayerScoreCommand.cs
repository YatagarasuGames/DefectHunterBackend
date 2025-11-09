using MediatR;

namespace Leaderboard.API.Services.Commands.SetPlayerScore
{
    public class SetPlayerScoreCommand : IRequest<Guid>
    {
        public Guid Id { get; }
        public uint NewScore { get; }
        public SetPlayerScoreCommand(Guid id, uint newScore)
        {
            Id = id;
            NewScore = newScore;
        }
    }
}
