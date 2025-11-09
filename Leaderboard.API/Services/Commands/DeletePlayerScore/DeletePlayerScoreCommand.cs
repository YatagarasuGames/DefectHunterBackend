using MediatR;

namespace Leaderboard.API.Services.Commands.DeletePlayerScore
{
    public class DeletePlayerScoreCommand : IRequest<Guid>
    {
        public Guid Id { get; }
        public DeletePlayerScoreCommand(Guid id)
        {
            Id = id;
        }
    }
}
