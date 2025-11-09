namespace Auth.API.Contracts
{
    public record UserAuthResponse(
        Guid Id,
        string Username,
        string Email
        );
}
