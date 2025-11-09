namespace Leaderboard.API.Contracts
{
    public record ScoreUpdateRequest(
        Guid UserId,
        uint ScoreToAdd
        );
}
