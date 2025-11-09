using Auth.API.Abstractions;
using Auth.API.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<List<UserAuthResponse>>> GetUsers()
        {
            var users = await _usersService.GetAllUsers();
            var response = users.Select(u => new UserAuthResponse(u.Id, u.Username, u.Email)).ToList();
            
            return Ok(response);
        }

        [HttpGet("get/{userId}")]
        public async Task<ActionResult<UserAuthResponse>> GetUserByIdAsync([FromRoute] Guid userId)
        {
            var user = await _usersService.GetUserByIdAsync(userId);
            if (user == null) return BadRequest("User with given Id not found!");

            return Ok(new UserAuthResponse(user.Id, user.Username, user.Email));
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Guid>> CreateUser([FromBody] UserAuthRequest request)
        {
            var (user, error) = Shared.Models.User.Create(
                Guid.NewGuid(),
                request.Username,
                request.Email,
                request.Password
                );

            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }

            var userId = await _usersService.CreateUser(user);
            return Ok(userId);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Guid>> UpdateUser(Guid id, [FromBody] UserAuthRequest request)
        {
            var userId = await _usersService.UpdateUser(id, request.Username, request.Email, request.Password);
            return Ok(userId);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Guid>> DeleteUser(Guid id)
        {
            var userId = await _usersService.DeleteUser(id);
            return Ok(userId);
        }

    }
}
