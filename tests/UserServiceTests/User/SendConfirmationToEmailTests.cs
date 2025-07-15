using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Text;
using UserService.Application.UseCases.Commands;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using Microsoft.AspNetCore.WebUtilities;
using Bogus;

namespace UserServiceTests
{
    public class SendConfirmationToEmailTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly SendConfirmationToEmailHandler _handler;
        private readonly Faker _faker;

        public SendConfirmationToEmailTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null
            );
            _emailServiceMock = new Mock<IEmailService>();
            _handler = new SendConfirmationToEmailHandler(_userManagerMock.Object, _emailServiceMock.Object);
            _faker = new Faker();
        }

        [Fact]
        public async Task Should_Send_Confirmation_Email_Successfully()
        {
            // Arrange
            var user = new User { Id = _faker.Random.Guid() };
            var email = _faker.Internet.Email();
            var url = _faker.Internet.Url();
            var confirmationCode = _faker.Random.AlphaNumeric(32);

            _userManagerMock.Setup(um => um.GenerateEmailConfirmationTokenAsync(user))
                .ReturnsAsync(confirmationCode);

            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationCode));
            var expectedUrl = $"{url}/User/confirmEmail?userId={user.Id}&code={encodedCode}";

            var command = new SendConfirmationToEmailCommand(user.Id, url, email);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _userManagerMock.Verify(um => um.GenerateEmailConfirmationTokenAsync(user), Times.Once);
            _emailServiceMock.Verify(es => es.SendConfirmationEmailAsync(
                email,
                $"<a href='{expectedUrl}'>Confirm</a>"
            ), Times.Once);
        }

        [Fact]
        public async Task Should_Send_Change_Email_Confirmation_Successfully()
        {
            // Arrange
            var user = new User { Id = _faker.Random.Guid() };
            var email = _faker.Internet.Email();
            var url = _faker.Internet.Url();
            var confirmationCode = _faker.Random.AlphaNumeric(32);

            _userManagerMock.Setup(um => um.GenerateChangeEmailTokenAsync(user, email))
                .ReturnsAsync(confirmationCode);

            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationCode));
            var expectedUrl = $"{url}/User/confirmEmail?userId={user.Id}&code={encodedCode}&changedEmail={email}";

            var command = new SendConfirmationToEmailCommand(user.Id, url, email, isChange: true);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _userManagerMock.Verify(um => um.GenerateChangeEmailTokenAsync(user, email), Times.Once);
            _emailServiceMock.Verify(es => es.SendConfirmationEmailAsync(
                email,
                $"<a href='{expectedUrl}'>Confirm</a>"
            ), Times.Once);
        }
    }
}
