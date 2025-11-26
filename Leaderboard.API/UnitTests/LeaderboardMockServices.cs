using Leaderboard.API.Services.Commands.AddPlayerScore;
using Leaderboard.API.Services.Commands.CreatePlayerScore;
using Leaderboard.API.Services.Commands.DeletePlayerScore;
using Leaderboard.API.Services.Commands.SetPlayerScore;
using Leaderboard.API.Services.Queries.GetAllPlayerScores;
using MediatR;
using Moq;
using Shared.Models;
using System.Security.Claims;

namespace Leaderboard.API.UnitTests
{
    public class LeaderboardMockServices
    {
        public static Mock<IMediator> CreateMediatorMock()
        {
            var mock = new Mock<IMediator>();

            // Настройка для GetAllPlayerScoresQuery
            mock.Setup(x => x.Send(It.IsAny<GetAllPlayerScoresQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PlayerScore>
                {
                    PlayerScore.Create(Guid.NewGuid(), "user1", 100).Value,
                    PlayerScore.Create(Guid.NewGuid(), "user2", 200).Value
                });

            // Настройка для CreatePlayerScoreCommand
            mock.Setup(x => x.Send(It.IsAny<CreatePlayerScoreCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            // Настройка для DeletePlayerScoreCommand
            mock.Setup(x => x.Send(It.IsAny<DeletePlayerScoreCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            // Настройка для AddPlayerScoreCommand
            mock.Setup(x => x.Send(It.IsAny<AddPlayerScoreCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            // Настройка для SetPlayerScoreCommand
            mock.Setup(x => x.Send(It.IsAny<SetPlayerScoreCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            return mock;
        }

        public static Mock<ILogger<T>> CreateLoggerMock<T>() where T : class
        {
            return new Mock<ILogger<T>>();
        }

        public static ClaimsPrincipal CreateClaimsPrincipal(Guid? userId = null)
        {
            var claims = new List<Claim>
            {
                new Claim("nameid", userId?.ToString() ?? Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Email, "test@test.com")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }
    }
}
