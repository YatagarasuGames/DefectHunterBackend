using MediatR;
using Shared.Database.Abstractions;
using Shared.Models;

namespace Leaderboard.API.Services.Commands.CreatePlayerScore
{
    public class CreatePlayerScoreCommandHandler : IRequestHandler<CreatePlayerScoreCommand, Guid>
    {
        private readonly IPlayerScoreRepository _playerScoreRepository;
        private readonly ILogger<CreatePlayerScoreCommandHandler> _logger;

        public CreatePlayerScoreCommandHandler(
            IPlayerScoreRepository playeScoreRepository, 
            ILogger<CreatePlayerScoreCommandHandler> logger)
        {
            _playerScoreRepository = playeScoreRepository;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreatePlayerScoreCommand request, CancellationToken cancellationToken)
        {
            var playerScoreCreateResult = PlayerScore.Create(request.Id, request.Username, request.Score);
            if (!playerScoreCreateResult.IsSuccess)
            {
                _logger.LogError($"Error on creating new player score for player with username {request.Username}: {playerScoreCreateResult.Error}");
                throw new InvalidOperationException($"Error on creating new player score for player with username {request.Username}: {playerScoreCreateResult.Error}");
            }
            var playerScore = playerScoreCreateResult.Value;
            return await _playerScoreRepository.Create(playerScore);
        }
    }
}
