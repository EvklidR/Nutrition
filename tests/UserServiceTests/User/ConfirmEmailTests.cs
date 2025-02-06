using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Identity;
using System.Text;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using UserService.Application.UseCases.Commands;
using UserService.Domain.Entities;
using Microsoft.AspNetCore.WebUtilities;
using Bogus;

namespace UserServiceTests
{
    public class ConfirmEmailHandlerTests
    {
        private readonly Faker _faker;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly ConfirmEmailHandler _handler;

        public ConfirmEmailHandlerTests()
        {
            _faker = new Faker();
            _tokenServiceMock = new Mock<ITokenService>();
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _handler = new ConfirmEmailHandler(_tokenServiceMock.Object, _userManagerMock.Object);
        }

        [Fact]
        public async Task Handler_Should_Throw_Unauthorized_When_User_Not_Found()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var code = _faker.Random.AlphaNumeric(32);
            var command = new ConfirmEmailCommand(userId, code, null);

            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Unauthorized>().WithMessage("User not found");
        }

        [Fact]
        public async Task Handler_Should_Confirm_Email_When_Code_Is_Valid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var inputCode = _faker.Random.AlphaNumeric(32);
            var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(inputCode));
            var user = new User { Id = userId };
            var command = new ConfirmEmailCommand(userId, code, null);

            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(um => um.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _userManagerMock.Verify(um => um.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Handler_Should_Throw_BadRequest_When_Token_Does_Not_Match()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var inputCode = _faker.Random.AlphaNumeric(32);
            var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(inputCode));
            var user = new User { Id = userId };
            var command = new ConfirmEmailCommand(userId, code, null);

            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(um => um.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequest>();
        }

        [Fact]
        public async Task Handler_Should_Change_Email_When_New_Email_Is_Provided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var newEmail = _faker.Internet.Email();
            var oldEmail = _faker.Internet.Email();
            var inputCode = _faker.Random.AlphaNumeric(32);
            var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(inputCode));
            var user = new User { Id = userId, Email = oldEmail };
            var command = new ConfirmEmailCommand(userId, code, newEmail);

            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(um => um.ChangeEmailAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.SetUserNameAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _userManagerMock.Verify(um => um.ChangeEmailAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _userManagerMock.Verify(um => um.SetUserNameAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }
    }
}
