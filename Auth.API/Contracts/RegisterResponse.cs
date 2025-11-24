namespace Auth.API.Contracts
{
    public record RegisterResponse(
        string? accessToken,
        string? refreshToken,
        int expiresIn
        );
}
