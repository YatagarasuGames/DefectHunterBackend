using Leaderboard.API.Services.Commands.CreatePlayerScore;
using Moq;
using Shared.Database.Abstractions;
using Shared.Models;
using Xunit;

namespace Leaderboard.API.UnitTests
{
    public class CreatePlayerScoreCommandHandlerTests
    {
        private readonly CreatePlayerScoreCommandHandler _handler;
        private readonly Mock<IPlayerScoreRepository> _repositoryMock;
        private readonly Mock<ILogger<CreatePlayerScoreCommandHandler>> _loggerMock;

        public CreatePlayerScoreCommandHandlerTests()
        {
            _repositoryMock = LeaderboardCommandMocks.CreatePlayerScoreRepositoryMock();
            _loggerMock = LeaderboardCommandMocks.CreateLoggerMock<CreatePlayerScoreCommandHandler>();
            _handler = new CreatePlayerScoreCommandHandler(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldReturnPlayerScoreId()
        {
            var command = new CreatePlayerScoreCommand(Guid.NewGuid(), "testuser", 100);
            var expectedId = Guid.NewGuid();

            _repositoryMock.Setup(x => x.Create(It.IsAny<PlayerScore>()))
                .ReturnsAsync(expectedId);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(expectedId, result);
            _repositoryMock.Verify(x => x.Create(It.IsAny<PlayerScore>()), Times.Once);
        }

        [Theory]
        [InlineData("validuser", 100u)]
        [InlineData("user123", 0u)]
        [InlineData("test-user", uint.MaxValue)]
        public async Task Handle_WithDifferentValidData_ShouldWork(string username, uint score)
        {
            var command = new CreatePlayerScoreCommand(Guid.NewGuid(), username, score);
            var expectedId = Guid.NewGuid();

            _repositoryMock.Setup(x => x.Create(It.IsAny<PlayerScore>()))
                .ReturnsAsync(expectedId);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.Equal(expectedId, result);
        }
    }
}
