namespace Auth.API.Contracts
{
    public record UpdateUserRequest(string Username, string Email, string Password);
}
