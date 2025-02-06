using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using UserService.Application.UseCases.Commands;
using UserService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace UserServiceTests
{
    public class RegisterUserTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly RegisterUserHandler _handler;
        private readonly Faker _faker;

        public RegisterUserTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null
            );
            _mediatorMock = new Mock<IMediator>();
            _tokenServiceMock = new Mock<ITokenService>();
            _handler = new RegisterUserHandler(_userManagerMock.Object, _mediatorMock.Object, _tokenServiceMock.Object);

            _faker = new Faker();
        }

        [Fact]
        public async Task Should_Register_User_Successfully()
        {
            // Arrange
            var command = new RegisterUserCommand(
                _faker.Internet.Email(),
                _faker.Internet.Password(),
                _faker.Internet.Url()
            );

            var user = new User();

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), command.password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), "user"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<User>(), command.password), Times.Once);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), "user"), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<SendConfirmationToEmailCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_BadRequest_When_Creation_Fails()
        {
            // Arrange
            var command = new RegisterUserCommand(
                _faker.Internet.Email(),
                _faker.Internet.Password(),
                null
            );

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), command.password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password is too weak" }));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            var errors = await act.Should().ThrowAsync<BadRequest>();
            errors.Which.Errors.Should().ContainSingle("Password is too weak");
        }

        [Fact]
        public async Task Should_Throw_BadRequest_When_AddToRole_Fails()
        {
            // Arrange
            var command = new RegisterUserCommand(
                _faker.Internet.Email(),
                _faker.Internet.Password(),
                null
            );

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), command.password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), "user"))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Role assignment failed" }));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            var errors = await act.Should().ThrowAsync<BadRequest>();
            errors.Which.Errors.Should().ContainSingle("Role assignment failed");
        }
    }
}
