using FluentAssertions;
using Moq;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using UserService.Application.UseCases.Commands;
using UserService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Bogus;

namespace UserServiceTests
{
    public class LoginUserTests
    {
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Faker _faker;

        public LoginUserTests()
        {
            _tokenServiceMock = new Mock<ITokenService>();
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null
            );
            _faker = new Faker();
        }

        [Fact]
        public async Task Handler_Should_Throw_Unauthorized_When_User_Not_Found()
        {
            // Arrange
            var command = new LoginUserCommand(_faker.Internet.Email(), _faker.Internet.Password());

            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            var handler = new LoginUserHandler(_tokenServiceMock.Object, _userManagerMock.Object);

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Unauthorized>().WithMessage("Login failed. Invalid email or password");
        }

        [Fact]
        public async Task Handler_Should_Throw_Unauthorized_When_Password_Is_Invalid()
        {
            // Arrange
            var user = new User { UserName = _faker.Internet.Email(), Email = _faker.Internet.Email() };
            var command = new LoginUserCommand(user.Email, _faker.Internet.Password());

            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var handler = new LoginUserHandler(_tokenServiceMock.Object, _userManagerMock.Object);

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Unauthorized>().WithMessage("Login failed. Invalid email or password");
        }

        [Fact]
        public async Task Handler_Should_Return_AuthenticatedResponse_When_Login_Succeeds()
        {
            // Arrange
            var user = new User { UserName = _faker.Internet.Email(), Email = _faker.Internet.Email() };
            var command = new LoginUserCommand(user.Email, _faker.Internet.Password());
            var accessToken = _faker.Random.AlphaNumeric(32);
            var refreshToken = _faker.Random.AlphaNumeric(32);

            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            _tokenServiceMock.Setup(ts => ts.GenerateAccessToken(It.IsAny<User>()))
                .ReturnsAsync(accessToken);
            _tokenServiceMock.Setup(ts => ts.GenerateRefreshToken(It.IsAny<User>()))
                .ReturnsAsync(refreshToken);

            var handler = new LoginUserHandler(_tokenServiceMock.Object, _userManagerMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().Be(accessToken);
            result.RefreshToken.Should().Be(refreshToken);
        }
    }
}
