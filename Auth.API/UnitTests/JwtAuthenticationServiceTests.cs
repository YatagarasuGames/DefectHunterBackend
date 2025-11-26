using Auth.API.Services;
using Moq;
using Shared.Database.Abstractions;
using Shared.Models;
using Xunit;

namespace Auth.API.UnitTests
{
    public class JwtAuthenticationServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IUsersRepository> _usersRepositoryMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly JwtAuthenticationService _jwtService;

        public JwtAuthenticationServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();

            SetupConfiguration();

            _jwtService = new JwtAuthenticationService(
                _configurationMock.Object,
                _usersRepositoryMock.Object,
                _refreshTokenRepositoryMock.Object
            );
        }

        private void SetupConfiguration()
        {
            // Mock для строковых значений
            _configurationMock.Setup(x => x["JwtConfig:Issuer"]).Returns("https://localhost:7000");
            _configurationMock.Setup(x => x["JwtConfig:Audience"]).Returns("https://localhost:7000");
            _configurationMock.Setup(x => x["JwtConfig:Key"]).Returns("b7f463b31b03cf19376c0e996402343aba949d31a419a75943d3a64513d2085319bcc05868ed40fbb42b4bae453bf25b18ab0f54dc9873b94a3c5b30f32d5a09");

            // Mock для GetValue<int> - создаем конфигурационную секцию
            var configSectionMock = new Mock<IConfigurationSection>();
            configSectionMock.Setup(x => x.Value).Returns("1");

            _configurationMock.Setup(x => x.GetSection("JwtConfig:TokenValidInMins"))
                .Returns(configSectionMock.Object);
        }

        [Fact]
        public async Task GenerateJwtToken_WithValidUserId_ShouldReturnLoginResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = User.Create(userId, "testuser", "test@email.com", "hashed_password").Value;

            _usersRepositoryMock.Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _jwtService.GenerateJwtToken(userId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.accessToken);
            Assert.NotNull(result.refreshToken);
            Assert.True(result.expiresIn > 0);
        }

        [Fact]
        public async Task GenerateJwtToken_WithNullUser_ShouldStillReturnResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _usersRepositoryMock.Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _jwtService.GenerateJwtToken(userId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.accessToken);
            Assert.NotNull(result.refreshToken);
        }

        [Fact]
        public async Task GenerateRefreshToken_ShouldReturnTokenString()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var refreshTokenId = Guid.NewGuid();

            _refreshTokenRepositoryMock.Setup(x => x.Create(It.IsAny<RefreshToken>()))
                .ReturnsAsync(refreshTokenId);

            // Act
            var result = await _jwtService.GenerateRefreshToken(userId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }


        [Fact]
        public async Task ValidateRefreshToken_WithExpiredToken_ShouldReturnNull()
        {
            // Arrange
            var expiredTokenResult = RefreshToken.Create(
                Guid.NewGuid(),
                "expired_token",
                DateTime.UtcNow.AddHours(-1), // expired
                Guid.NewGuid()
            );

            _refreshTokenRepositoryMock.Setup(x => x.GetTokenByToken("expired_token"))
                .ReturnsAsync(expiredTokenResult.Value);

            // Act
            var result = await _jwtService.ValidateRefreshToken("expired_token");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ValidateRefreshToken_WithNonExistentToken_ShouldReturnNull()
        {
            // Arrange
            _refreshTokenRepositoryMock.Setup(x => x.GetTokenByToken("nonexistent_token"))
                .ReturnsAsync((RefreshToken)null);

            // Act
            var result = await _jwtService.ValidateRefreshToken("nonexistent_token");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ValidateRefreshToken_WithNullUser_ShouldReturnNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var refreshTokenResult = RefreshToken.Create(
                Guid.NewGuid(),
                "valid_token_but_no_user",
                DateTime.UtcNow.AddHours(1),
                userId
            );

            _refreshTokenRepositoryMock.Setup(x => x.GetTokenByToken("valid_token_but_no_user"))
                .ReturnsAsync(refreshTokenResult.Value);
            _refreshTokenRepositoryMock.Setup(x => x.DeleteById(It.IsAny<Guid>()))
                .ReturnsAsync(Guid.NewGuid());
            _usersRepositoryMock.Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _jwtService.ValidateRefreshToken("valid_token_but_no_user");

            // Assert
            Assert.Null(result);
        }
    }
}
