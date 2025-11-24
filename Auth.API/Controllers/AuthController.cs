using Auth.API.Abstractions;
using Auth.API.Contracts;
using Auth.API.Services.Commands.CreateUser;
using Auth.API.Services.Queries.GetUserByEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Auth.API.Controllers
{
    
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {

        private IPasswordService _passwordService;
        private IMediator _mediator;
        private readonly IRabbitMQService _rabbitmqService;
        private readonly IJwtAuthenticationService _jwtAuthenticationService;
        private ILogger<AuthController> _logger;

        public AuthController(IPasswordService passwordService,
            IMediator mediator,
            ILogger<AuthController> logger,
            IRabbitMQService rabbitmqService,
            IJwtAuthenticationService jwtAuthenticationService
            )
        {
            _passwordService = passwordService;
            _mediator = mediator;
            _logger = logger;
            _rabbitmqService = rabbitmqService;
            _jwtAuthenticationService = jwtAuthenticationService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<Guid>> Register([FromBody] RegisterRequest request)
        {

            try
            {
                var userId = await _mediator.Send(new CreateUserCommand(
                    Guid.NewGuid(),
                    request.username,
                    request.email,
                    _passwordService.HashPassword(request.password)
                    ));

                var response = await _jwtAuthenticationService.GenerateJwtToken(userId);
                _rabbitmqService.PublishUserRegisteredEvent(new Shared.Events.UserRegisteredEvent(userId, request.username, request.email));
                _logger.LogInformation("User registered and event published: {UserId}", userId);

                return Ok(response);
            }

            catch (Exception ex)
            {

                return BadRequest(new ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<Guid>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var existingUser = await _mediator.Send(new GetUserByEmailQuery(request.email));

                if (existingUser == null) return BadRequest(new ErrorResponse("User with given email not found"));

                if (_passwordService.VerifyPassword(request.password, existingUser.Password))
                {
                    var response = await _jwtAuthenticationService.GenerateJwtToken(existingUser.Id);
                    return Ok(response);
                }
                else
                {
                    return BadRequest(new ErrorResponse("Wrong credentials"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email {Email}", request.email);
                return StatusCode(500, new ErrorResponse("Internal server error"));
            }
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] RefreshRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.token))
                    return BadRequest("Refresh token is required");

                var loginResponse = await _jwtAuthenticationService.ValidateRefreshToken(request.token);

                if (loginResponse == null)
                    return Unauthorized("Invalid or expired refresh token");

                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
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
