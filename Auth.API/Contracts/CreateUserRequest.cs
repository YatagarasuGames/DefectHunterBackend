namespace Auth.API.Controllers
{
    public record CreateUserRequest(string Username, string Email, string Password);
}
