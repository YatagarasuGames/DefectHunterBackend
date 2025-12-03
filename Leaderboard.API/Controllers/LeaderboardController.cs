using Leaderboard.API.Contracts;
using Leaderboard.API.Services.Commands.AddPlayerScore;
using Leaderboard.API.Services.Commands.CreatePlayerScore;
using Leaderboard.API.Services.Commands.DeletePlayerScore;
using Leaderboard.API.Services.Commands.SetPlayerScore;
using Leaderboard.API.Services.Queries.GetAllPlayerScores;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using System.Security.Claims;

namespace Leaderboard.API.Controllers
{
    [ApiController]
    [Route("api/leaderboard")]
    [Authorize]
    public class LeaderboardController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<LeaderboardController> _logger;
        public LeaderboardController(IMediator mediator, ILogger<LeaderboardController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("get-all-users")]
        public async Task<ActionResult<List<PlayerScore>>> GetLeaderboard()
        {
            try
            {
                var leaderboard = await _mediator.Send(new GetAllPlayerScoresQuery());
                _logger.LogDebug("Gettint all player scores successfully");
                return Ok(leaderboard);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost("create-user-score")]
        public async Task<ActionResult<Guid>> CreatePlayerScore([FromBody] ScoreCreateRequest request)
        {
            try
            {
                var id = await _mediator.Send(new CreatePlayerScoreCommand(request.UserId, request.Username, request.Score));
                _logger.LogDebug($"Creating player score for userId {request.UserId} is done");
                return Ok(id);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("delete-player-score/{requestId}")]
        [SwaggerOnly]
        public async Task<ActionResult<Guid>> DeletePlayerScore([FromRoute] Guid requestId)
        {
            try
            {
                var id = await _mediator.Send(new DeletePlayerScoreCommand(requestId));
                return Ok(id);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("add-to-player-score")]
        public async Task<ActionResult<Guid>> AddToPlayerScore([FromBody] ScoreUpdateRequest request)
        {
            try
            {
                var id = await _mediator.Send(new AddPlayerScoreCommand(request.UserId, request.ScoreToAdd));
                return Ok(id);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("replace-player-score")]
        [SwaggerOnly]
        public async Task<ActionResult<Guid>> ReplacePlayerScore([FromBody] ScoreUpdateRequest request)
        {
            try
            {
                var id = await _mediator.Send(new SetPlayerScoreCommand(request.UserId, request.ScoreToAdd));
                return Ok(id);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
