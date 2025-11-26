using Auth.API.Abstractions;
using Auth.API.Contracts;
using MediatR;
using Moq;
using Shared.Events;

namespace Auth.API.UnitTests
{
    public class MockServices
    {
        public static Mock<IPasswordService> CreatePasswordServiceMock()
        {
            var mock = new Mock<IPasswordService>();
            mock.Setup(x => x.HashPassword(It.IsAny<string>()))
                .Returns((string password) => $"hashed_{password}");
            mock.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string password, string hash) => hash == $"hashed_{password}");
            return mock;
        }

        public static Mock<IMediator> CreateMediatorMock()
        {
            return new Mock<IMediator>();
        }

        public static Mock<ILogger<T>> CreateLoggerMock<T>()
        {
            return new Mock<ILogger<T>>();
        }

        public static Mock<IRabbitMQService> CreateRabbitMQServiceMock()
        {
            var mock = new Mock<IRabbitMQService>();
            mock.Setup(x => x.PublishUserRegisteredEvent(It.IsAny<UserRegisteredEvent>()));
            return mock;
        }

        public static Mock<IJwtAuthenticationService> CreateJwtAuthenticationServiceMock()
        {
            var mock = new Mock<IJwtAuthenticationService>();
            mock.Setup(x => x.GenerateJwtToken(It.IsAny<Guid>()))
                .ReturnsAsync(new LoginResponse("test_token", "refresh_token", 3600));
            mock.Setup(x => x.ValidateRefreshToken(It.IsAny<string>()))
                .ReturnsAsync(new LoginResponse("refreshed_token", "new_refresh_token", 3600));
            return mock;
        }
    }
}
