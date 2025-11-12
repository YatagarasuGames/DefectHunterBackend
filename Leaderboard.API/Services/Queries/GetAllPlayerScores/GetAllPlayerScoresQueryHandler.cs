using MediatR;
using Shared.Database.Abstractions;
using Shared.Database.Repositories;
using Shared.Models;

namespace Leaderboard.API.Services.Queries.GetAllPlayerScores
{
    public class GetAllPlayerScoresQueryHandler : IRequestHandler<GetAllPlayerScoresQuery, List<PlayerScore>>
    {
        private readonly IPlayerScoreRepository _repository;
        private readonly ILogger<GetAllPlayerScoresQueryHandler> _logger;

        public GetAllPlayerScoresQueryHandler(IPlayerScoreRepository repository, ILogger<GetAllPlayerScoresQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<List<PlayerScore>> Handle(GetAllPlayerScoresQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Getting all players scores");

            var result = await _repository.GetAllScores();

            if(result ==  null)
            {
                _logger.LogError("Error on getting all player scores: result is null");
                throw new NullReferenceException("Error on getting all player scores: result is null");
            }

            return result;
        }
    }
}
