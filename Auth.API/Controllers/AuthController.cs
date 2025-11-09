using Auth.API.Abstractions;
using Auth.API.Contracts;
using Leaderboard.API.Contracts;
using Microsoft.AspNetCore.Mvc;
using Shared.Database.Abstractions;
using Shared.Models;
using System.Net.Http;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {

        private IPasswordService _passwordService;
        private IUsersService _usersService;
        private ILogger _logger;

        public AuthController(IPasswordService passwordService, IUsersService usersService, ILogger logger)
        {
            _passwordService = passwordService;
            _usersService = usersService;
            _logger = logger;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<Guid>> Register([FromBody] UserAuthRequest request)
        {
            var users = await _usersService.GetAllUsers();
            if(users.FirstOrDefault(u => u.Email == request.Email) != null)
            {
                return BadRequest("User with same email already registered");
            }
            else
            {
                var result = Shared.Models.User.Create(
                    Guid.NewGuid(),
                    request.Username,
                    request.Email,
                    _passwordService.HashPassword(request.Password)
                    );
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Error);
                }

                var userId = await _usersService.CreateUser(result.Value);
                HttpClient _httpClient = new HttpClient();

                ScoreCreateRequest scoreRequest = new ScoreCreateRequest(userId, result.Value.Username, 0);
                var response = await _httpClient.PostAsJsonAsync($"https://localhost:7160/api/leaderboard/create", scoreRequest);
                if (!response.IsSuccessStatusCode)
                {
                    // Логируем ошибку, но не прерываем регистрацию
                    Console.WriteLine($"Failed to create player score: {response.StatusCode}");
                }
                else
                {
                    Console.WriteLine($"Success to create player score: {response.StatusCode}");
                }
                    return Ok(result.Value.Id);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromBody] UserAuthRequest request)
        {
            var users = await _usersService.GetAllUsers();

            var existingUser = users.FirstOrDefault(u => u.Email == request.Email);
            if(existingUser == null)
            {
                return BadRequest("User with this email does not exist");
            }

            if(_passwordService.VerifyPassword(request.Password, existingUser.Password))
            {
                return Ok("success");
            }
            else
            {
                return BadRequest("Wrong credits");
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
