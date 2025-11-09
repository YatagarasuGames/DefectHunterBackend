using Leaderboard.API.Services.Commands.CreatePlayerScore;
using MediatR;
using Shared.Database.Abstractions;

namespace Leaderboard.API.Services.Commands.AddPlayerScore
{
    public class AddPlayerScoreCommandHandler : IRequestHandler<AddPlayerScoreCommand, Guid>
    {
        private readonly IPlayerScoreRepository _playerScoreRepository;
        private readonly ILogger<AddPlayerScoreCommandHandler> _logger;

        public AddPlayerScoreCommandHandler(
            IPlayerScoreRepository playeScoreRepository,
            ILogger<AddPlayerScoreCommandHandler> logger)
        {
            _playerScoreRepository = playeScoreRepository;
            _logger = logger;
        }

        public async Task<Guid> Handle(AddPlayerScoreCommand request, CancellationToken cancellationToken)
        {
            var result = await _playerScoreRepository.GetScoreByUserId(request.Id);
            if(result == null)
            {
                _logger.LogError($"Player score with id {request.Id} not found");
                throw new ArgumentException($"Player score with id {request.Id} not found");
            }
            return await _playerScoreRepository.AddScore(request.Id, request.Score);
        }
    }
}
