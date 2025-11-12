using Leaderboard.API.Services.Queries.GetAllPlayerScores;
using MediatR;
using Shared.Database.Abstractions;
using Shared.Models;

namespace Leaderboard.API.Services.Queries.GetPlayerScoreByUserId
{
    public class GetPlayerScoreByUserIdQueryHandler : IRequestHandler<GetPlayerScoreByUserIdQuery, PlayerScore>
    {
        private readonly IPlayerScoreRepository _repository;
        private readonly ILogger<GetAllPlayerScoresQueryHandler> _logger;

        public GetPlayerScoreByUserIdQueryHandler(IPlayerScoreRepository repository, ILogger<GetAllPlayerScoresQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PlayerScore> Handle(GetPlayerScoreByUserIdQuery request, CancellationToken cancellationToken)
        {
            var playerScore = await _repository.GetScoreByUserId(request.Id);
            if (playerScore == null)
            {
                _logger.LogError($"Error on getting player score for user with id: {request.Id}");
                throw new NullReferenceException($"Error on getting player score for user with id: {request.Id}");
            }
            return playerScore;
        }
    }
}
