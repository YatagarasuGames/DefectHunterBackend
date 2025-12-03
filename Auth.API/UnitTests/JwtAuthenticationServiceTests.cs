using Auth.API.Services;
using DotNetEnv;
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
            Env.Load();

            _configurationMock.Setup(x => x["JwtConfig:Issuer"]).Returns("JwtConfig:Issuer");
            _configurationMock.Setup(x => x["JwtConfig:Audience"]).Returns("JwtConfig:Audience");
            _configurationMock.Setup(x => x["JwtConfig:Key"]).Returns("JwtConfig:Key");

            var configSectionMock = new Mock<IConfigurationSection>();
            configSectionMock.Setup(x => x.Value).Returns("1");

            _configurationMock.Setup(x => x.GetSection("JwtConfig:TokenValidInMins"))
                .Returns(configSectionMock.Object);
        }

        [Fact]
        public async Task GenerateRefreshToken_ShouldReturnTokenString()
        {
            var userId = Guid.NewGuid();
            var refreshTokenId = Guid.NewGuid();

            _refreshTokenRepositoryMock.Setup(x => x.Create(It.IsAny<RefreshToken>()))
                .ReturnsAsync(refreshTokenId);

            var result = await _jwtService.GenerateRefreshToken(userId);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }


        [Fact]
        public async Task ValidateRefreshToken_WithExpiredToken_ShouldReturnNull()
        {
            var expiredTokenResult = RefreshToken.Create(
                Guid.NewGuid(),
                "expired_token",
                DateTime.UtcNow.AddHours(-1), // expired
                Guid.NewGuid()
            );

            _refreshTokenRepositoryMock.Setup(x => x.GetTokenByToken("expired_token"))
                .ReturnsAsync(expiredTokenResult.Value);

            var result = await _jwtService.ValidateRefreshToken("expired_token");

            Assert.Null(result);
        }

        [Fact]
        public async Task ValidateRefreshToken_WithNonExistentToken_ShouldReturnNull()
        {
            _refreshTokenRepositoryMock.Setup(x => x.GetTokenByToken("nonexistent_token"))
                .ReturnsAsync((RefreshToken)null);

            var result = await _jwtService.ValidateRefreshToken("nonexistent_token");

            Assert.Null(result);
        }

        [Fact]
        public async Task ValidateRefreshToken_WithNullUser_ShouldReturnNull()
        {
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

            var result = await _jwtService.ValidateRefreshToken("valid_token_but_no_user");

            Assert.Null(result);
        }
    }
}
