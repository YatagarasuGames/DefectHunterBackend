using Auth.API.Abstractions;
using Auth.API.Contracts;
using Auth.API.Services.Commands.CreateUser;
using Auth.API.Services.Commands.DeleteUser;
using Auth.API.Services.Commands.UpdateUser;
using Auth.API.Services.Queries.GetAllUsers;
using Auth.API.Services.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;
        private readonly IPasswordService _passwordService;

        public UsersController(
            IMediator mediator,
            ILogger<UsersController> logger,
            IPasswordService passwordService)
        {
            _logger = logger;
            _mediator = mediator;
            _passwordService = passwordService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetUsers()
        {
            try
            {
                var users = await _mediator.Send(new GetAllUsersQuery());
                var response = users.Select(u => new UserResponse(u.Id, u.Username, u.Email)).ToList();

                _logger.LogInformation("Retrieved {UserCount} users", users.Count);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return BadRequest(new ErrorResponse("Failed to retrieve users"));
            }
        }

        [HttpGet("{userId:guid}")]
        public async Task<ActionResult<UserResponse>> GetUserById([FromRoute] Guid userId)
        {
            try
            {

                var user = await _mediator.Send(new GetUserByIdQuery(userId));
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", userId);
                    return NotFound(new ErrorResponse("User not found"));
                }

                var response = new UserResponse(user.Id, user.Username, user.Email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", userId);
                return BadRequest(new ErrorResponse("Failed to retrieve user"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse("Invalid request data"));
                }

                var userId = Guid.NewGuid();
                var hashedPassword = _passwordService.HashPassword(request.Password);

                var createdUserId = await _mediator.Send(new CreateUserCommand(
                    userId,
                    request.Username,
                    request.Email,
                    hashedPassword
                ));

                _logger.LogInformation("User created with ID: {UserId}, Email: {Email}", createdUserId, request.Email);
                return Ok(createdUserId);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                _logger.LogWarning("Attempt to create user with existing email: {Email}", request.Email);
                return Conflict(new ErrorResponse("User with this email already exists"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user with email {Email}", request.Email);
                return BadRequest(new ErrorResponse("Failed to create user"));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Guid>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                if (!CanAccessUser(id))
                {
                    return Forbid("You can only update your own user data");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse("Invalid request data"));
                }

                string hashedPassword = null;
                if (!string.IsNullOrEmpty(request.Password))
                {
                    hashedPassword = _passwordService.HashPassword(request.Password);
                }

                var userId = await _mediator.Send(new UpdateUserCommand(
                    id,
                    request.Username,
                    request.Email,
                    hashedPassword
                ));

                _logger.LogInformation("User updated with ID: {UserId}", userId);
                return Ok(userId);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "User with ID {UserId} not found for update", id);
                return NotFound(new ErrorResponse("User not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user with ID {UserId}", id);
                return BadRequest(new ErrorResponse("Failed to update user"));
            }
        }


        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Guid>> DeleteUser(Guid id)
        {
            try
            {
                var userId = await _mediator.Send(new DeleteUserCommand(id));

                _logger.LogInformation("User deleted with ID: {UserId}", userId);
                return Ok(userId);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "User with ID {UserId} not found for deletion", id);
                return NotFound(new ErrorResponse("User not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user with ID {UserId}", id);
                return BadRequest(new ErrorResponse("Failed to delete user"));
            }
        }

        private bool CanAccessUser(Guid targetUserId)
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            return currentUserId == targetUserId || isAdmin;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("nameid") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("Invalid user ID in token");
        }
    }
}
