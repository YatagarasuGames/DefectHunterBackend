using Auth.API.Abstractions;
using Auth.API.Contracts;
using Auth.API.Services.Commands.CreateUser;
using Auth.API.Services.Queries.GetUserByEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers
{
    public record ErrorResponse(string message);
    public record RegisterResponse(Guid id);
    public class LoginResponse
    {
        public Guid Id { get; }
        public string? AccessToken { get; }
        public string? RefreshToken { get; }
        public int ExpiresIn { get; }

        public LoginResponse(Guid id, string accessToken, string refreshToken, int expiresIn)
        {
            Id = id;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpiresIn = expiresIn;
        }
    }
    
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {

        private IPasswordService _passwordService;
        private IMediator _mediator;
        private readonly IRabbitMQService _rabbitmqService;
        private ILogger<AuthController> _logger;

        public AuthController(IPasswordService passwordService, IMediator mediator, ILogger<AuthController> logger, IRabbitMQService rabbitmqService)
        {
            _passwordService = passwordService;
            _mediator = mediator;
            _logger = logger;
            _rabbitmqService = rabbitmqService;
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
                _rabbitmqService.PublishUserRegisteredEvent(new Shared.Events.UserRegisteredEvent(userId, request.Username, request.Email));
                _logger.LogInformation("User registered and event published: {UserId}", userId);
                return Ok(new RegisterResponse(userId));
            }

            catch (Exception ex)
            {

                return BadRequest(new ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<Guid>> Login([FromBody] UserAuthRequest request)
        {
            try
            {
                var existingUser = await _mediator.Send(new GetUserByEmailQuery(request.Email));

                if (_passwordService.VerifyPassword(request.Password, existingUser.Password))
                {
                    return Ok("gfgf");//new LoginResponse(existingUser.Id));
                }
                else
                {
                    return BadRequest(new ErrorResponse("Wrong credentials"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email {Email}", request.Email);
                return StatusCode(500, new ErrorResponse("Internal server error"));
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
