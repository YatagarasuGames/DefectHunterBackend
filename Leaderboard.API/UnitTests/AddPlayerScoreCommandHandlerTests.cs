using Leaderboard.API.Services.Commands.AddPlayerScore;
using Moq;
using Shared.Database.Abstractions;
using Shared.Models;
using Xunit;

namespace Leaderboard.API.UnitTests
{
    public class AddPlayerScoreCommandHandlerTests
    {
        private readonly AddPlayerScoreCommandHandler _handler;
        private readonly Mock<IPlayerScoreRepository> _repositoryMock;
        private readonly Mock<ILogger<AddPlayerScoreCommandHandler>> _loggerMock;

        public AddPlayerScoreCommandHandlerTests()
        {
            _repositoryMock = LeaderboardCommandMocks.CreatePlayerScoreRepositoryMock();
            _loggerMock = LeaderboardCommandMocks.CreateLoggerMock<AddPlayerScoreCommandHandler>();
            _handler = new AddPlayerScoreCommandHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_WithExistingPlayer_ShouldReturnPlayerId()
        {
            var userId = Guid.NewGuid();
            var scoreToAdd = 50u;
            var command = new AddPlayerScoreCommand(userId, scoreToAdd);
            var existingPlayer = PlayerScore.Create(userId, "testuser", 100).Value;
            var expectedId = Guid.NewGuid();

            _repositoryMock.Setup(x => x.GetScoreByUserId(userId))
                .ReturnsAsync(existingPlayer);
            _repositoryMock.Setup(x => x.AddScore(userId, scoreToAdd))
                .ReturnsAsync(expectedId);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(expectedId, result);
            _repositoryMock.Verify(x => x.GetScoreByUserId(userId), Times.Once);
            _repositoryMock.Verify(x => x.AddScore(userId, scoreToAdd), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentPlayer_ShouldThrowException()
        {

            var nonExistentUserId = Guid.NewGuid();
            var command = new AddPlayerScoreCommand(nonExistentUserId, 50);

            _repositoryMock.Setup(x => x.GetScoreByUserId(nonExistentUserId))
                .ReturnsAsync((PlayerScore)null);


            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Contains($"Player score with id {nonExistentUserId} not found", exception.Message);
        }

        [Fact]
        public async Task Handle_WithNonExistentPlayer_ShouldLogError()
        {

            var nonExistentUserId = Guid.NewGuid();
            var command = new AddPlayerScoreCommand(nonExistentUserId, 50);

            _repositoryMock.Setup(x => x.GetScoreByUserId(nonExistentUserId))
                .ReturnsAsync((PlayerScore)null);


            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Player score with id {nonExistentUserId} not found")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Theory]
        [InlineData(1u)]
        [InlineData(100u)]
        [InlineData(999u)]
        public async Task Handle_WithDifferentScores_ShouldWork(uint scoreToAdd)
        {
            var userId = Guid.NewGuid();
            var command = new AddPlayerScoreCommand(userId, scoreToAdd);
            var existingPlayer = PlayerScore.Create(userId, "testuser", 100).Value;

            _repositoryMock.Setup(x => x.GetScoreByUserId(userId))
                .ReturnsAsync(existingPlayer);
            _repositoryMock.Setup(x => x.AddScore(userId, scoreToAdd))
                .ReturnsAsync(userId);


            var result = await _handler.Handle(command, CancellationToken.None);


            Assert.Equal(userId, result);
        }
    }
}
