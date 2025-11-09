using Auth.API.Abstractions;
using Auth.API.Contracts;
using Auth.API.Services.Commands.CreateUser;
using Auth.API.Services.Queries.GetUserByEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {

        private IPasswordService _passwordService;
        private IMediator _mediator;
        private ILogger<AuthController> _logger;

        public AuthController(IPasswordService passwordService, IMediator mediator, ILogger<AuthController> logger)
        {
            _passwordService = passwordService;
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<Guid>> Register([FromBody] UserAuthRequest request)
        {

            try
            {
                var userId = await _mediator.Send(new CreateUserCommand(
                    Guid.NewGuid(),
                    request.Username,
                    request.Email,
                    _passwordService.HashPassword(request.Password)
                    ));
                _logger.LogInformation("User registered and event published: {UserId}", userId);
                return Ok(userId);
            }

            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] UserAuthRequest request)
        {
            try
            {
                var existingUser = await _mediator.Send(new GetUserByEmailQuery(request.Email));

                if (_passwordService.VerifyPassword(request.Password, existingUser.Password))
                {
                    return Ok("success");
                }
                else
                {
                    return BadRequest("Wrong credentials");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email {Email}", request.Email);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        [Route("test")]
        public ActionResult TestConnection()
        {
            return Ok("Server is running");
        }
    }
}
