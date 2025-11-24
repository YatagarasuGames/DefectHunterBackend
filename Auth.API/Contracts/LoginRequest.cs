namespace Auth.API.Contracts
{
    public record LoginRequest(
        string email,
        string password
        );
}
