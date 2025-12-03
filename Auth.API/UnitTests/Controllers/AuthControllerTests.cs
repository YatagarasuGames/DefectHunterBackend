using Auth.API.Abstractions;
using Auth.API.Contracts;
using Auth.API.Controllers;
using Auth.API.Services;
using Auth.API.Services.Commands.CreateUser;
using Auth.API.Services.Queries.GetUserByEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Events;
using Shared.Models;
using Xunit;

namespace Auth.API.UnitTests.Controllers
{
    public class AuthControllerTests
    {
        private readonly AuthController _authController;
        private readonly Mock<IPasswordService> _passwordServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly Mock<IRabbitMQService> _rabbitMQServiceMock;
        private readonly Mock<IJwtAuthenticationService> _jwtAuthenticationServiceMock;

        public AuthControllerTests()
        {
            _passwordServiceMock = MockServices.CreatePasswordServiceMock();
            _mediatorMock = MockServices.CreateMediatorMock();
            _loggerMock = MockServices.CreateLoggerMock<AuthController>();
            _rabbitMQServiceMock = MockServices.CreateRabbitMQServiceMock();
            _jwtAuthenticationServiceMock = MockServices.CreateJwtAuthenticationServiceMock();

            _authController = new AuthController(
                _passwordServiceMock.Object,
                _mediatorMock.Object,
                _loggerMock.Object,
                _rabbitMQServiceMock.Object,
                _jwtAuthenticationServiceMock.Object
            );
        }

        [Fact]
        public async Task Register_WithValidRequest_ShouldReturnOk()
        {
            var userId = Guid.NewGuid();
            var request = new RegisterRequest("testuser", "test@email.com", "password123");

            _mediatorMock.Setup(x => x.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userId);

            var result = await _authController.Register(request);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            _mediatorMock.Verify(x => x.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _rabbitMQServiceMock.Verify(x => x.PublishUserRegisteredEvent(It.IsAny<UserRegisteredEvent>()), Times.Once);
        }

        [Fact]
        public async Task Register_WithExistingEmail_ShouldReturnBadRequest()
        {
            var request = new RegisterRequest("testuser", "existing@email.com", "password123");

            _mediatorMock.Setup(x => x.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("User with email existing@email.com already exists"));

            var result = await _authController.Register(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Contains("already exists", errorResponse.message);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnOk()
        {
            var request = new LoginRequest("test@email.com", "password123");
            var user = User.Create(Guid.NewGuid(), "testuser", "test@email.com", "hashed_password123").Value;

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _passwordServiceMock.Setup(x => x.VerifyPassword(request.password, user.Password))
                .Returns(true);

            var result = await _authController.Login(request);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Login_WithInvalidEmail_ShouldReturnBadRequest()
        {
            var request = new LoginRequest("nonexistent@email.com", "password123");

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            var result = await _authController.Login(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("User with given email not found", errorResponse.message);
        }

        [Fact]
        public async Task Login_WithWrongPassword_ShouldReturnBadRequest()
        {
            var request = new LoginRequest("test@email.com", "wrongpassword");
            var user = User.Create(Guid.NewGuid(), "testuser", "test@email.com", "hashed_password123").Value;

            _mediatorMock.Setup(x => x.Send(It.IsAny<GetUserByEmailQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _passwordServiceMock.Setup(x => x.VerifyPassword(request.password, user.Password))
                .Returns(false);

            var result = await _authController.Login(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errorResponse = Assert.IsType<ErrorResponse>(badRequestResult.Value);
            Assert.Equal("Wrong credentials", errorResponse.message);
        }

        [Fact]
        public async Task RefreshToken_WithValidToken_ShouldReturnOk()
        {
            var request = new RefreshRequest("valid_refresh_token");

            var result = await _authController.RefreshToken(request);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task RefreshToken_WithEmptyToken_ShouldReturnBadRequest()
        {
            var request = new RefreshRequest("");

            var result = await _authController.RefreshToken(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Refresh token is required", badRequestResult.Value);
        }

        [Fact]
        public async Task RefreshToken_WithInvalidToken_ShouldReturnUnauthorized()
        {
            var request = new RefreshRequest("invalid_token");
            _jwtAuthenticationServiceMock.Setup(x => x.ValidateRefreshToken(It.IsAny<string>()))
                .ReturnsAsync((LoginResponse)null);

            var result = await _authController.RefreshToken(request);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid or expired refresh token", unauthorizedResult.Value);
        }

        [Fact]
        public void TestConnection_ShouldReturnOk()
        {
            var result = _authController.TestConnection();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Server is running", okResult.Value);
        }
    }
}