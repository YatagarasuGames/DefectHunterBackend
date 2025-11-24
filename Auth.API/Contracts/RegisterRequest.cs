namespace Auth.API.Contracts
{
    public record RegisterRequest(string username, string email, string password);
}
