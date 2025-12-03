using Auth.API.Services.Commands.CreateUser;
using Auth.API.Services.Commands.DeleteUser;
using Moq;
using Shared.Database.Abstractions;
using Shared.Models;
using Xunit;

namespace Auth.API.UnitTests
{
    public class CreateUserCommandHandlerTests
    {
        [Fact]
        public async Task Handle_WithNewUser_ShouldReturnUserId()
        {
            var userId = Guid.NewGuid();
            var command = new CreateUserCommand(userId, "testuser", "test@email.com", "hashed_password");

            var usersRepositoryMock = new Mock<IUsersRepository>();
            var loggerMock = MockServices.CreateLoggerMock<CreateUserCommandHandler>();

            usersRepositoryMock.Setup(x => x.GetUserByEmailAsync(command.Email))
                .ReturnsAsync((User)null);
            usersRepositoryMock.Setup(x => x.Create(It.IsAny<User>()))
                .ReturnsAsync(userId);

            var handler = new CreateUserCommandHandler(usersRepositoryMock.Object, loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.Equal(userId, result);
        }

        [Fact]
        public async Task Handle_WithExistingEmail_ShouldThrowException()
        {
            var command = new CreateUserCommand(Guid.NewGuid(), "testuser", "existing@email.com", "hashed_password");
            var existingUser = User.Create(Guid.NewGuid(), "existinguser", "existing@email.com", "hashed_password").Value;

            var usersRepositoryMock = new Mock<IUsersRepository>();
            var loggerMock = MockServices.CreateLoggerMock<CreateUserCommandHandler>();

            usersRepositoryMock.Setup(x => x.GetUserByEmailAsync(command.Email))
                .ReturnsAsync(existingUser);

            var handler = new CreateUserCommandHandler(usersRepositoryMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(command, CancellationToken.None));
        }
    }

    public class DeleteUserCommandHandlerTests
    {
        [Fact]
        public async Task Handle_WithExistingUser_ShouldReturnUserId()
        {
            var userId = Guid.NewGuid();
            var command = new DeleteUserCommand(userId);
            var user = User.Create(userId, "testuser", "test@email.com", "hashed_password").Value;

            var usersRepositoryMock = new Mock<IUsersRepository>();
            var loggerMock = MockServices.CreateLoggerMock<DeleteUserCommandHandler>();

            usersRepositoryMock.Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);
            usersRepositoryMock.Setup(x => x.Delete(userId))
                .ReturnsAsync(userId);

            var handler = new DeleteUserCommandHandler(usersRepositoryMock.Object, loggerMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.Equal(userId, result);
        }

        [Fact]
        public async Task Handle_WithNonExistentUser_ShouldThrowException()
        {
            var userId = Guid.NewGuid();
            var command = new DeleteUserCommand(userId);

            var usersRepositoryMock = new Mock<IUsersRepository>();
            var loggerMock = MockServices.CreateLoggerMock<DeleteUserCommandHandler>();

            usersRepositoryMock.Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            var handler = new DeleteUserCommandHandler(usersRepositoryMock.Object, loggerMock.Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                handler.Handle(command, CancellationToken.None));
        }
    }
}
