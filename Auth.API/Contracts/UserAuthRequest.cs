namespace Auth.API.Contracts
{
    public record UserAuthRequest(
        string Username,
        string Email,
        string Password
        );
}
