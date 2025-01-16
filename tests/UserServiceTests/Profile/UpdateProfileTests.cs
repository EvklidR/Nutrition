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
        private readonly Faker<UpdateProfileDTO> _updateProfileDTOFaker;
        private readonly Faker _faker;
        private readonly Mock<IProfileRepository> _profileRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly UpdateProfileHandler _handler;

        public UpdateProfileTests()
        {
            _updateProfileDTOFaker = new Faker<UpdateProfileDTO>()
                .RuleFor(p => p.Id, f => f.Random.Guid())
                .RuleFor(p => p.Name, f => f.Name.FullName())
                .RuleFor(p => p.Weight, f => f.Random.Double(50, 100))
                .RuleFor(p => p.Height, f => f.Random.Double(150, 200))
                .RuleFor(p => p.ActivityLevel, f => f.PickRandom<ActivityLevel>())
                .RuleFor(p => p.DesiredGlassesOfWater, f => f.Random.Int(5, 12));

            _faker = new Faker();

            _profileRepositoryMock = new Mock<IProfileRepository>();
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new UpdateProfileHandler(_profileRepositoryMock.Object, _mapperMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task Handler_Should_Throw_Unauthorized_When_User_Is_Not_The_Owner_Of_The_Profile()
        {
            // Arrange
            var profileDto = _updateProfileDTOFaker.Generate();
            var userId = _faker.Random.Guid();

            _profileRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new UserService.Domain.Entities.Profile { UserId = _faker.Random.Guid() });

            _mapperMock.Setup(m => m.Map<UserService.Domain.Entities.Profile>(profileDto))
                .Returns(new UserService.Domain.Entities.Profile());

            var command = new UpdateProfileCommand(profileDto, userId);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Unauthorized>().WithMessage("Owner isn't valid");
        }

        [Fact]
        public async Task Handler_Should_Throw_NotFound_When_Profile_Is_Not_Found()
        {
            // Arrange
            var profileDto = _updateProfileDTOFaker.Generate();
            var userId = _faker.Random.Guid();

            _profileRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((UserService.Domain.Entities.Profile)null);

            var command = new UpdateProfileCommand(profileDto, userId);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFound>().WithMessage("Profile not found.");
        }

        [Fact]
        public async Task Handler_Should_Throw_AlreadyExists_When_Another_Profile_With_Same_Name_Exists()
        {
            // Arrange
            var profileDto = _updateProfileDTOFaker.Generate();
            var userId = _faker.Random.Guid();

            var existingProfiles = new List<UserService.Domain.Entities.Profile>
            {
                new UserService.Domain.Entities.Profile { UserId = userId, Name = profileDto.Name }
            };

            _profileRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new UserService.Domain.Entities.Profile { UserId = userId });

            _profileRepositoryMock.Setup(repo => repo.GetAllByUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingProfiles);

            _mapperMock.Setup(m => m.Map<UserService.Domain.Entities.Profile>(profileDto))
                .Returns(new UserService.Domain.Entities.Profile { Name = profileDto.Name });

            var command = new UpdateProfileCommand(profileDto, userId);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<AlreadyExists>().WithMessage("Profile with this name in your account already exists");
        }

        [Fact]
        public async Task Handler_Should_Update_Profile_When_Valid_Command_Is_Provided()
        {
            // Arrange
            var profileDto = _updateProfileDTOFaker.Generate();
            var userId = _faker.Random.Guid();

            var existingProfile = new UserService.Domain.Entities.Profile { UserId = userId, Name = "Old Name" };

            _profileRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingProfile);

            _profileRepositoryMock.Setup(repo => repo.GetAllByUserAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<UserService.Domain.Entities.Profile>());

            _mapperMock.Setup(m => m.Map(It.IsAny<UpdateProfileDTO>(), It.IsAny<UserService.Domain.Entities.Profile>()));

            var command = new UpdateProfileCommand(profileDto, userId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _profileRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map(It.IsAny<UpdateProfileDTO>(), It.IsAny<UserService.Domain.Entities.Profile>()), Times.Once);
        }
    }
}
