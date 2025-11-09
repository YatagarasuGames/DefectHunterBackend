namespace Leaderboard.API.Contracts
{
    public record ScoreCreateRequest(
        Guid UserId,
        string Username,
        uint Score
        );
}
