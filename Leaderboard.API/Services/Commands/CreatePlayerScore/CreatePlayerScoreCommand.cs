using MediatR;

namespace Leaderboard.API.Services.Commands.CreatePlayerScore
{
    public class CreatePlayerScoreCommand : IRequest<Guid>
    {
        public Guid Id { get; }
        public string Username { get; }
        public uint Score { get; }
        public CreatePlayerScoreCommand(Guid id, string username, uint score)
        {
            Id = id;
            Username = username;
            Score = score;
        }
    }
}
