namespace Shared.Events
{
    public record UserRegisteredEvent(
        Guid UserId,
        string Username,
        string Email
    );
}
