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
            var password = "TestPassword123";

            var hashed = _passwordService.HashPassword(password);

            Assert.NotNull(hashed);
            Assert.NotEqual(password, hashed);
            Assert.True(hashed.Length > 0);
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
        {
            var password = "TestPassword123";
            var hashed = _passwordService.HashPassword(password);

            var result = _passwordService.VerifyPassword(password, hashed);

            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithWrongPassword_ShouldReturnFalse()
        {
            var password = "TestPassword123";
            var wrongPassword = "WrongPassword123";
            var hashed = _passwordService.HashPassword(password);

            var result = _passwordService.VerifyPassword(wrongPassword, hashed);

            Assert.False(result);
        }
    }
}