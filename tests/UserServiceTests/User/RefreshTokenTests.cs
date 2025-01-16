using FluentAssertions;
using Moq;
using System.Security.Claims;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using UserService.Application.UseCases.Commands;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using Bogus;

namespace UserServiceTests
{
    public class RefreshTokenHandlerTests
    {
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IRefreshTokenTokenRepository> _refreshTokenRepositoryMock;
        private readonly Faker _faker;
        private readonly RefreshTokenHandler _handler;

        public RefreshTokenHandlerTests()
        {
            _tokenServiceMock = new Mock<ITokenService>();
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null
            );
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenTokenRepository>();
            _faker = new Faker();

            _handler = new RefreshTokenHandler(
                _userManagerMock.Object,
                _tokenServiceMock.Object,
                _refreshTokenRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Should_Throw_Unauthorized_When_User_Not_Found()
        {
            // Arrange
            var command = new RefreshTokenCommand(
                _faker.Random.AlphaNumeric(32),
                _faker.Random.AlphaNumeric(32)
            );

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, _faker.Internet.Email())
            }));

            _tokenServiceMock.Setup(ts => ts.GetPrincipalFromExpiredToken(It.IsAny<string>()))
                .Returns(principal);

            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Unauthorized>().WithMessage("User not found");
        }

        [Fact]
        public async Task Should_Throw_Unauthorized_When_RefreshToken_Invalid()
        {
            // Arrange
            var user = new User { Email = _faker.Internet.Email(), Id = Guid.NewGuid() };
            var command = new RefreshTokenCommand(
                _faker.Random.AlphaNumeric(32),
                _faker.Random.AlphaNumeric(32)
            );

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Email)
            }));

            _tokenServiceMock.Setup(ts => ts.GetPrincipalFromExpiredToken(It.IsAny<string>()))
                .Returns(principal);

            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _refreshTokenRepositoryMock.Setup(repo => repo.GetAllByUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<RefreshToken>());

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Unauthorized>().WithMessage("There is no refresh token");
        }

        [Fact]
        public async Task Should_Return_AuthenticatedResponse_When_Tokens_Valid()
        {
            // Arrange
            var user = new User { Email = _faker.Internet.Email(), Id = Guid.NewGuid() };
            var refreshToken = _faker.Random.AlphaNumeric(32);
            var command = new RefreshTokenCommand(
                _faker.Random.AlphaNumeric(32),
                refreshToken
            );

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Email)
            }));

            _tokenServiceMock.Setup(ts => ts.GetPrincipalFromExpiredToken(It.IsAny<string>()))
                .Returns(principal);

            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _refreshTokenRepositoryMock.Setup(repo => repo.GetAllByUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<RefreshToken>
                {
                    new RefreshToken { Token = refreshToken }
                });

            var newAccessToken = _faker.Random.AlphaNumeric(32);

            _tokenServiceMock.Setup(ts => ts.GenerateAccessToken(user))
                .ReturnsAsync(newAccessToken);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().Be(newAccessToken);
            result.RefreshToken.Should().Be(refreshToken);
        }
    }
}
