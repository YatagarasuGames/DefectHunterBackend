namespace Auth.API.Contracts
{
    public record LoginResponse
    (
        string? accessToken,
        string? refreshToken,
        int expiresIn
    );
}
