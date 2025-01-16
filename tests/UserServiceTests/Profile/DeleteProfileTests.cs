using Bogus;
using FluentAssertions;
using Moq;
using UserService.Application.Exceptions;
using UserService.Application.UseCases.Commands;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;

namespace UserServiceTests
{
    public class DeleteProfileTests
    {
        private readonly Faker _faker;
        private readonly Mock<IProfileRepository> _profileRepositoryMock;
        private DeleteProfileHandler _handler;

        public DeleteProfileTests()
        {
            _faker = new Faker();
            _profileRepositoryMock = new Mock<IProfileRepository>();
            _handler = new DeleteProfileHandler(_profileRepositoryMock.Object);
        }

        [Fact]
        public async Task Handler_Should_Throw_NotFound_When_Profile_Does_Not_Exist()
        {
            // Arrange
            _profileRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Profile)null);

            var command = new DeleteProfileCommand(_faker.Random.Guid(), _faker.Random.Guid());

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFound>().WithMessage("Profile not found");
        }

        [Fact]
        public async Task Handler_Should_Throw_Unauthorized_When_UserId_Does_Not_Match_Profile_UserId()
        {
            // Arrange
            var profile = new Profile { UserId = _faker.Random.Guid() };

            _profileRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(profile);

            var command = new DeleteProfileCommand(_faker.Random.Guid(), _faker.Random.Guid());

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Unauthorized>().WithMessage("Owner isn't valid");
        }

        [Fact]
        public async Task Handler_Should_Delete_Profile_When_Valid_Command_Is_Provided()
        {
            // Arrange
            var profileId = _faker.Random.Guid();
            var userId = _faker.Random.Guid();

            var profile = new Profile { UserId = userId };

            _profileRepositoryMock.Setup(repo => repo.GetByIdAsync(profileId))
                .ReturnsAsync(profile);

            var command = new DeleteProfileCommand(profileId, userId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _profileRepositoryMock.Verify(repo => repo.Delete(profile), Times.Once);
            _profileRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}
