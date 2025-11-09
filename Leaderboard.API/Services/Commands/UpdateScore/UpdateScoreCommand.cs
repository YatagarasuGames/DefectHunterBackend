using MediatR;

namespace Leaderboard.API.Services.Commands.UpdateScore
{
    public class UpdateScoreCommand : IRequest<Guid>
    {
        public Guid Id { get; }
        public string Username { get; }
        public uint Score { get; }

        public UpdateScoreCommand(Guid id, string username, uint score)
        {
            Id = id;
            Username = username;
            Score = score;
        }
    }
}
