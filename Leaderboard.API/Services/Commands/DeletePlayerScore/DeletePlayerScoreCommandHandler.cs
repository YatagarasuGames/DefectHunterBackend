using Leaderboard.API.Services.Commands.CreatePlayerScore;
using MediatR;
using Shared.Database.Abstractions;

namespace Leaderboard.API.Services.Commands.DeletePlayerScore
{
    public class DeletePlayerScoreCommandHandler : IRequestHandler<DeletePlayerScoreCommand, Guid>
    {
        private readonly IPlayerScoreRepository _playerScoreRepository;
        private readonly ILogger<DeletePlayerScoreCommandHandler> _logger;

        public DeletePlayerScoreCommandHandler(
            IPlayerScoreRepository playeScoreRepository,
            ILogger<DeletePlayerScoreCommandHandler> logger)
        {
            _playerScoreRepository = playeScoreRepository;
            _logger = logger;
        }

        public async Task<Guid> Handle(DeletePlayerScoreCommand request, CancellationToken cancellationToken)
        {
            var playerScore = await _playerScoreRepository.GetScoreByUserId(request.Id);
            if (playerScore == null)
            {
                _logger.LogError($"Player score of user with id {request.Id} not found");
                throw new ArgumentException($"Player score of user with id {request.Id} not found");
            }
            return await _playerScoreRepository.Delete(request.Id);


        }
    }
}
