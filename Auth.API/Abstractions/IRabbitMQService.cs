using Shared.Events;

namespace Auth.API.Abstractions
{
    public interface IRabbitMQService
    {
        void PublishUserRegisteredEvent(UserRegisteredEvent request);
    }
}