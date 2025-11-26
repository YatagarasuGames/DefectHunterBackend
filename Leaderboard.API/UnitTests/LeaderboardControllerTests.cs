using Leaderboard.API.Contracts;
using Leaderboard.API.Controllers;
using Leaderboard.API.Services.Commands.AddPlayerScore;
using Leaderboard.API.Services.Commands.CreatePlayerScore;
using Leaderboard.API.Services.Commands.DeletePlayerScore;
using Leaderboard.API.Services.Commands.SetPlayerScore;
using Leaderboard.API.Services.Queries.GetAllPlayerScores;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Models;
using System.Security.Claims;
using Xunit;

namespace Leaderboard.API.UnitTests
{
    public class LeaderboardControllerTests
    {
        private readonly LeaderboardController _controller;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<LeaderboardController>> _loggerMock;

        public LeaderboardControllerTests()
        {
            _mediatorMock = LeaderboardMockServices.CreateMediatorMock();
            _loggerMock = LeaderboardMockServices.CreateLoggerMock<LeaderboardController>();

            _controller = new LeaderboardController(
                _mediatorMock.Object,
                _loggerMock.Object
            );

            // Устанавливаем контекст пользователя с валидными claims
            var user = LeaderboardMockServices.CreateClaimsPrincipal();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetLeaderboard_ShouldReturnOkWithScores()
        {
            // Arrange
            var expectedScores = new List<PlayerScore>
            {
                PlayerScore.Create(Guid.NewGuid(), "user1", 100).Value,
                PlayerScore.Create(Guid.NewGuid(), "user2", 200).Value,
                PlayerScore.Create(Guid.NewGuid(), "user3", 150).Value
            };

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetAllPlayerScoresQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedScores);

            // Act
            var result = await _controller.GetLeaderboard();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedScores = Assert.IsType<List<PlayerScore>>(okResult.Value);
            Assert.Equal(expectedScores.Count, returnedScores.Count);

            _mediatorMock.Verify(x => x.Send(It.IsAny<GetAllPlayerScoresQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetLeaderboard_WhenExceptionThrown_ShouldReturnBadRequest()
        {
            // Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetAllPlayerScoresQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetLeaderboard();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Database error", badRequestResult.Value);
        }

        [Fact]
        public async Task CreatePlayerScore_WithValidRequest_ShouldReturnOk()
        {
            // Arrange
            var request = new ScoreCreateRequest(Guid.NewGuid(), "testuser", 100);
            var expectedId = Guid.NewGuid();

            _mediatorMock.Setup(x => x.Send(It.IsAny<CreatePlayerScoreCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _controller.CreatePlayerScore(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedId, okResult.Value);

            _mediatorMock.Verify(x => x.Send(
                It.Is<CreatePlayerScoreCommand>(cmd =>
                    cmd.Id == request.UserId &&
                    cmd.Username == request.Username &&
                    cmd.Score == request.Score),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreatePlayerScore_WhenExceptionThrown_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new ScoreCreateRequest(Guid.NewGuid(), "testuser", 100);

            _mediatorMock.Setup(x => x.Send(It.IsAny<CreatePlayerScoreCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("User already exists"));

            // Act
            var result = await _controller.CreatePlayerScore(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("User already exists", badRequestResult.Value);
        }

        [Fact]
        public async Task DeletePlayerScore_WithValidId_ShouldReturnOk()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var expectedId = Guid.NewGuid();

            _mediatorMock.Setup(x => x.Send(It.IsAny<DeletePlayerScoreCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _controller.DeletePlayerScore(requestId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedId, okResult.Value);

            _mediatorMock.Verify(x => x.Send(
                It.Is<DeletePlayerScoreCommand>(cmd => cmd.Id == requestId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeletePlayerScore_WhenExceptionThrown_ShouldReturnBadRequest()
        {
            // Arrange
            var requestId = Guid.NewGuid();

            _mediatorMock.Setup(x => x.Send(It.IsAny<DeletePlayerScoreCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Score not found"));

            // Act
            var result = await _controller.DeletePlayerScore(requestId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Score not found", badRequestResult.Value);
        }

        [Fact]
        public async Task AddToPlayerScore_WithValidRequest_ShouldReturnOk()
        {
            // Arrange
            var request = new ScoreUpdateRequest(Guid.NewGuid(), 50);
            var expectedId = Guid.NewGuid();

            _mediatorMock.Setup(x => x.Send(It.IsAny<AddPlayerScoreCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _controller.AddToPlayerScore(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedId, okResult.Value);

            _mediatorMock.Verify(x => x.Send(
                It.Is<AddPlayerScoreCommand>(cmd =>
                    cmd.Id == request.UserId &&
                    cmd.Score == request.ScoreToAdd),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ReplacePlayerScore_WithValidRequest_ShouldReturnOk()
        {
            // Arrange
            var request = new ScoreUpdateRequest(Guid.NewGuid(), 300);
            var expectedId = Guid.NewGuid();

            _mediatorMock.Setup(x => x.Send(It.IsAny<SetPlayerScoreCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _controller.ReplacePlayerScore(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedId, okResult.Value);

            _mediatorMock.Verify(x => x.Send(
                It.Is<SetPlayerScoreCommand>(cmd =>
                    cmd.Id == request.UserId &&
                    cmd.NewScore == request.ScoreToAdd),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void GetUserIdFromToken_WithValidNameIdClaim_ShouldReturnUserId()
        {
            // Arrange
            var expectedUserId = Guid.NewGuid();
            var user = LeaderboardMockServices.CreateClaimsPrincipal(expectedUserId);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var method = typeof(LeaderboardController).GetMethod("GetUserIdFromToken", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var result = (Guid)method.Invoke(_controller, null);

            // Assert
            Assert.Equal(expectedUserId, result);
        }
    }
}
