using MediatR;

namespace Leaderboard.API.Services.Commands.AddPlayerScore
{
    public class AddPlayerScoreCommand : IRequest<Guid>
    {
        public Guid Id { get; }
        public uint Score { get; }

        public AddPlayerScoreCommand(Guid id, uint score)
        {
            Id = id;
            Score = score;
        }
    }
}
