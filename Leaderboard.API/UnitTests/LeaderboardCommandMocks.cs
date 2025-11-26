using Moq;
using Shared.Database.Abstractions;
using Shared.Models;

namespace Leaderboard.API.UnitTests
{
    public static class LeaderboardCommandMocks
    {
        public static Mock<IPlayerScoreRepository> CreatePlayerScoreRepositoryMock()
        {
            var mock = new Mock<IPlayerScoreRepository>();
            var testUserId = Guid.NewGuid();
            var testPlayerScore = PlayerScore.Create(testUserId, "testuser", 100).Value;

            // Настройка для GetScoreByUserId
            mock.Setup(x => x.GetScoreByUserId(testUserId))
                .ReturnsAsync(testPlayerScore);

            mock.Setup(x => x.GetScoreByUserId(It.Is<Guid>(id => id != testUserId)))
                .ReturnsAsync((PlayerScore)null);

            // Настройка для Create
            mock.Setup(x => x.Create(It.IsAny<PlayerScore>()))
                .ReturnsAsync(Guid.NewGuid());

            // Настройка для Delete
            mock.Setup(x => x.Delete(testUserId))
                .ReturnsAsync(testUserId);

            // Настройка для AddScore
            mock.Setup(x => x.AddScore(testUserId, It.IsAny<uint>()))
                .ReturnsAsync(testUserId);

            // Настройка для SetScore
            mock.Setup(x => x.SetScore(testUserId, It.IsAny<uint>()))
                .ReturnsAsync(testUserId);

            // Настройка для Update
            mock.Setup(x => x.Update(testUserId, It.IsAny<string>(), It.IsAny<uint>()))
                .ReturnsAsync(testUserId);

            return mock;
        }

        public static Mock<ILogger<T>> CreateLoggerMock<T>() where T : class
        {
            return new Mock<ILogger<T>>();
        }
    }
}
