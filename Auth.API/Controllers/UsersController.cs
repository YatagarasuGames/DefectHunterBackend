using Auth.API.Abstractions;
using Auth.API.Contracts;
using Auth.API.Services;
using Auth.API.Services.Commands.CreateUser;
using Auth.API.Services.Commands.DeleteUser;
using Auth.API.Services.Commands.UpdateUser;
using Auth.API.Services.Queries.GetAllUsers;
using Auth.API.Services.Queries.GetUserById;
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;
        private readonly IPasswordService _passwordService;

        public UsersController(IMediator mediator, ILogger<UsersController> logger, IPasswordService passwordService)
        {
            _logger = logger;
            _mediator = mediator;
            _passwordService = passwordService;
        }

        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<List<UserAuthResponse>>> GetUsers()
        {
            try
            {
                var users = await _mediator.Send(new GetAllUsersQuery());
                var response = users.Select(u => new UserAuthResponse(u.Id, u.Username, u.Email)).ToList();
                _logger.LogInformation("Get all users");
                return Ok(response);
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error on getting all users: {ex.Message}");
                return BadRequest(ex.Message);  
            }

        }

        [HttpGet("get/{userId}")]
        public async Task<ActionResult<UserAuthResponse>> GetUserByIdAsync([FromRoute] Guid userId)
        {
            try
            {
                var user = await _mediator.Send(new GetUserByIdQuery(userId));
                if (user == null) return BadRequest("User with given Id not found!");

                return Ok(new UserAuthResponse(user.Id, user.Username, user.Email));
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error on getting user with id {userId}: {ex.Message}");
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Guid>> CreateUser([FromBody] UserAuthRequest request)
        {
            try
            {
                var userId = await _mediator.Send(new CreateUserCommand(
                    Guid.NewGuid(),
                    request.Username,
                    request.Email,
                    _passwordService.HashPassword(request.Password)
                    ));

                _logger.LogInformation("User created: {UserId}", userId);
                return Ok(userId);
            }

            catch (Exception ex)
            {
                _logger.LogError($"Failed to create user with email {request.Email}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Guid>> UpdateUser(Guid id, [FromBody] UserAuthRequest request)
        {
            try
            {
                var userId = await _mediator.Send(new UpdateUserCommand(id, request.Username, request.Email, request.Password));
                _logger.LogInformation($"Update user with id {id} and email {request.Email}");
                return Ok(userId);
            }

            catch (Exception ex)
            {
                _logger.LogError($"Failed to update user with id {id} and email {request.Email}: {ex.Message}");
                return BadRequest(ex.Message);
            }
            
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Guid>> DeleteUser(Guid id)
        {
            try
            {
                var userId = await _mediator.Send(new DeleteUserCommand(id));
                _logger.LogInformation($"Delete user with id {id}");
                return Ok(userId);
            }

            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete user with id {id}: {ex.Message}");
                return BadRequest(ex.Message);
            }

        }

    }
}
