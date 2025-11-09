using Leaderboard.API.Services.Commands.SetPlayerScore;
using MediatR;
using Shared.Database.Abstractions;

namespace Leaderboard.API.Services.Commands.UpdateScore
{
    public class UpdateScoreCommandHandler : IRequestHandler<UpdateScoreCommand, Guid>
    {
        private readonly IPlayerScoreRepository _playerScoreRepository;
        private readonly ILogger<UpdateScoreCommandHandler> _logger;

        public UpdateScoreCommandHandler(
            IPlayerScoreRepository playeScoreRepository,
            ILogger<UpdateScoreCommandHandler> logger)
        {
            _playerScoreRepository = playeScoreRepository;
            _logger = logger;
        }

        public async Task<Guid> Handle(UpdateScoreCommand request, CancellationToken cancellationToken)
        {
            var result = await _playerScoreRepository.GetScoreByUserId(request.Id);
            if (result == null)
            {
                _logger.LogError($"Player score with id {request.Id} not found");
                throw new ArgumentException($"Player score with id {request.Id} not found");
            }
            return await _playerScoreRepository.Update(request.Id, request.Username, request.Score);
        }
    }
}
