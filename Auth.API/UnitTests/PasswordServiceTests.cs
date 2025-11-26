using Auth.API.Services;
using Xunit;

namespace Auth.API.UnitTests.Services
{
    public class PasswordServiceTests
    {
        private readonly PasswordService _passwordService;

        public PasswordServiceTests()
        {
            _passwordService = new PasswordService();
        }

        [Fact]
        public void HashPassword_ShouldReturnHashedPassword()
        {
            // Arrange
            var password = "TestPassword123";

            // Act
            var hashed = _passwordService.HashPassword(password);

            // Assert
            Assert.NotNull(hashed);
            Assert.NotEqual(password, hashed);
            Assert.True(hashed.Length > 0);
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            var password = "TestPassword123";
            var hashed = _passwordService.HashPassword(password);

            // Act
            var result = _passwordService.VerifyPassword(password, hashed);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithWrongPassword_ShouldReturnFalse()
        {
            // Arrange
            var password = "TestPassword123";
            var wrongPassword = "WrongPassword123";
            var hashed = _passwordService.HashPassword(password);

            // Act
            var result = _passwordService.VerifyPassword(wrongPassword, hashed);

            // Assert
            Assert.False(result);
        }
    }
}