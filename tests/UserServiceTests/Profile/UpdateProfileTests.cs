using AutoMapper;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.DTOs;
using UserService.Application.Exceptions;
using UserService.Application.UseCases.Commands;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Repositories;

namespace UserServiceTests
{
    public class UpdateProfileTests
    {
        private readonly Faker _faker;

        public UpdateProfileTests()
        {
            _faker = new Faker();
        }

        private UpdateProfileDTO GenerateUpdateProfileDTO()
        {
            return new UpdateProfileDTO
            {
                Id = _faker.Random.Guid(),
                Name = _faker.Name.FullName(),
                Weight = _faker.Random.Double(50, 100),
                Height = _faker.Random.Double(150, 200),
                ActivityLevel = _faker.PickRandom<ActivityLevel>(),
                DesiredGlassesOfWater = _faker.Random.Int(5, 12)
            };
        }

        [Fact]
        public async Task Handler_Should_Throw_Unauthorized_When_User_Is_Not_The_Owner_Of_The_Profile()
        {
            // Arrange
            var profileDto = GenerateUpdateProfileDTO();

            var userId = _faker.Random.Guid();

            var profileRepositoryMock = new Mock<IProfileRepository>();
            profileRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new UserService.Domain.Entities.Profile { UserId = _faker.Random.Guid() });

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<UserService.Domain.Entities.Profile>(profileDto))
                .Returns(new UserService.Domain.Entities.Profile());

            var handler = new UpdateProfileHandler(profileRepositoryMock.Object, mapperMock.Object, Mock.Of<IMediator>());
            var command = new UpdateProfileCommand(profileDto, userId);

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Unauthorized>().WithMessage("Owner isn't valid");
        }

        [Fact]
        public async Task Handler_Should_Throw_NotFound_When_Profile_Is_Not_Found()
        {
            // Arrange
            var profileDto = GenerateUpdateProfileDTO();

            var userId = _faker.Random.Guid();

            var profileRepositoryMock = new Mock<IProfileRepository>();
            profileRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((UserService.Domain.Entities.Profile)null);

            var mapperMock = new Mock<IMapper>();
            var handler = new UpdateProfileHandler(profileRepositoryMock.Object, mapperMock.Object, Mock.Of<IMediator>());
            var command = new UpdateProfileCommand(profileDto, userId);

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFound>().WithMessage("Profile not found.");
        }

        [Fact]
        public async Task Handler_Should_Throw_AlreadyExists_When_Another_Profile_With_Same_Name_Exists()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var profileDto = GenerateUpdateProfileDTO();

            var existingProfiles = new List<UserService.Domain.Entities.Profile>
            {
                new UserService.Domain.Entities.Profile { UserId = userId, Name = profileDto.Name }
            };

            var profileRepositoryMock = new Mock<IProfileRepository>();
            profileRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new UserService.Domain.Entities.Profile { UserId = userId });

            profileRepositoryMock.Setup(repo => repo.GetAllByUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingProfiles);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<UserService.Domain.Entities.Profile>(profileDto))
                .Returns(new UserService.Domain.Entities.Profile { Name = profileDto.Name });

            var handler = new UpdateProfileHandler(profileRepositoryMock.Object, mapperMock.Object, Mock.Of<IMediator>());
            var command = new UpdateProfileCommand(profileDto, userId);

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<AlreadyExists>().WithMessage("Profile with this name in your account already exists");
        }

        [Fact]
        public async Task Handler_Should_Update_Profile_When_Valid_Command_Is_Provided()
        {
            // Arrange
            var profileDto = GenerateUpdateProfileDTO();

            var userId = _faker.Random.Guid();

            var profileRepositoryMock = new Mock<IProfileRepository>();
            profileRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new UserService.Domain.Entities.Profile { UserId = userId, Name = "Old Name" });

            profileRepositoryMock.Setup(repo => repo.GetAllByUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<UserService.Domain.Entities.Profile>());

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map(It.IsAny<UpdateProfileDTO>(), It.IsAny<UserService.Domain.Entities.Profile>()));

            var mediatorMock = new Mock<IMediator>();

            var handler = new UpdateProfileHandler(profileRepositoryMock.Object, mapperMock.Object, mediatorMock.Object);
            var command = new UpdateProfileCommand(profileDto, userId);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            profileRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            mapperMock.Verify(m => m.Map(It.IsAny<UpdateProfileDTO>(), It.IsAny<UserService.Domain.Entities.Profile>()), Times.Once);
        }
    }
}
