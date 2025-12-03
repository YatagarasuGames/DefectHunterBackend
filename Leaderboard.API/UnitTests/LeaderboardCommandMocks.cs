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

            mock.Setup(x => x.GetScoreByUserId(testUserId))
                .ReturnsAsync(testPlayerScore);

            mock.Setup(x => x.GetScoreByUserId(It.Is<Guid>(id => id != testUserId)))
                .ReturnsAsync((PlayerScore)null);

            mock.Setup(x => x.Create(It.IsAny<PlayerScore>()))
                .ReturnsAsync(Guid.NewGuid());

            mock.Setup(x => x.Delete(testUserId))
                .ReturnsAsync(testUserId);

            mock.Setup(x => x.AddScore(testUserId, It.IsAny<uint>()))
                .ReturnsAsync(testUserId);

            mock.Setup(x => x.SetScore(testUserId, It.IsAny<uint>()))
                .ReturnsAsync(testUserId);

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
