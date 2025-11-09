using Leaderboard.API.Abstractions;
using Leaderboard.API.Contracts;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Leaderboard.API.Controllers
{
    [ApiController]
    [Route("api/leaderboard")]
    public class LeaderboardController : ControllerBase
    {
        private ILeaderboardService _leaderboardService;
        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet("get")]
        public async Task<ActionResult<List<PlayerScore>>> GetLeaderboard()
        {
            var leaderboard = await _leaderboardService.GetAllPlayerScores();
            return Ok(leaderboard);
        }

        [HttpPost("update")]
        public async Task<ActionResult<Guid>> UpdateUserScoreOnly([FromBody] ScoreUpdateRequest request)
        {

            return await _leaderboardService.UpdateOnlyPlayerScore(request.UserId, request.ScoreToAdd);
        }

        //[HttpPost("create")]
        //public async Task<ActionResult<Guid>> CreateUserScore([FromBody] ScoreCreateRequest request)
        //{
        //    return await _leaderboardService.CreatePlayerScore(PlayerScore.Create(request.UserId, request.Username, request.Score).PlayerScore);
        //}

    }
}
