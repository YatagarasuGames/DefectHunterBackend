using Leaderboard.API.Services.Commands.AddPlayerScore;
using MediatR;
using Shared.Database.Abstractions;

namespace Leaderboard.API.Services.Commands.SetPlayerScore
{
    public class SetPlayerScoreCommandHandler : IRequestHandler<SetPlayerScoreCommand, Guid>
    {
        private readonly IPlayerScoreRepository _playerScoreRepository;
        private readonly ILogger<SetPlayerScoreCommandHandler> _logger;

        public SetPlayerScoreCommandHandler(
            IPlayerScoreRepository playeScoreRepository,
            ILogger<SetPlayerScoreCommandHandler> logger)
        {
            _playerScoreRepository = playeScoreRepository;
            _logger = logger;
        }

        public async Task<Guid> Handle(SetPlayerScoreCommand request, CancellationToken cancellationToken)
        {
            var result = await _playerScoreRepository.GetScoreByUserId(request.Id);
            if (result == null)
            {
                _logger.LogError($"Player score with id {request.Id} not found");
                throw new ArgumentException($"Player score with id {request.Id} not found");
            }
            return await _playerScoreRepository.SetScore(request.Id, request.NewScore);
        }
    }
}
