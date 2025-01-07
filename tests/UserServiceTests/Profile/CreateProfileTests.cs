using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using UserService.Application.DTOs;
using UserService.Application.Exceptions;
using UserService.Application.UseCases.Commands;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Repositories;

namespace UserServiceTests
{
    public class CreateProfileTests
    {
        private readonly Faker _faker;

        public CreateProfileTests()
        {
            _faker = new Faker();
        }

        [Fact]
        public async Task Handler_Should_Throw_Unauthorized_When_User_Does_Not_Exist()
        {
            // Arrange
            var profileDto = new CreateProfileDTO
            {
                UserId = _faker.Random.Guid(),
                Name = _faker.Name.FullName(),
                Weight = _faker.Random.Double(50, 100),
                Height = _faker.Random.Double(150, 200),
                Birthday = DateOnly.FromDateTime(_faker.Date.Past(30, DateTime.Today.AddYears(-18))),
                Gender = Gender.Male,
                ActivityLevel = ActivityLevel.low
            };

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            var profileRepositoryMock = new Mock<IProfileRepository>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<UserService.Domain.Entities.Profile>(profileDto))
                .Returns(new UserService.Domain.Entities.Profile { UserId = (Guid)profileDto.UserId });

            var handler = new CreateProfileHandler(profileRepositoryMock.Object, userManagerMock.Object, mapperMock.Object);
            var command = new CreateProfileCommand(profileDto);

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Unauthorized>().WithMessage("User does not exist");
        }

        [Fact]
        public async Task Handler_Should_Throw_AlreadyExists_When_Profile_With_Same_Name_Exists()
        {
            // Arrange
            var profileDto = new CreateProfileDTO
            {
                UserId = _faker.Random.Guid(),
                Name = _faker.Name.FirstName(),
                Weight = _faker.Random.Double(50, 100),
                Height = _faker.Random.Double(150, 200),
                Birthday = DateOnly.FromDateTime(_faker.Date.Past(30, DateTime.Today.AddYears(-18))),
                Gender = Gender.Male,
                ActivityLevel = ActivityLevel.low
            };

            var existingProfiles = new List<UserService.Domain.Entities.Profile>
                {
                    new UserService.Domain.Entities.Profile { UserId = (Guid)profileDto.UserId, Name = profileDto.Name }
                };

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            var profileRepositoryMock = new Mock<IProfileRepository>();
            profileRepositoryMock.Setup(repo => repo.GetAllByUserAsync((Guid)profileDto.UserId))
                .ReturnsAsync(existingProfiles);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<UserService.Domain.Entities.Profile>(profileDto))
                .Returns(new UserService.Domain.Entities.Profile { UserId = (Guid)profileDto.UserId, Name = profileDto.Name });

            var handler = new CreateProfileHandler(profileRepositoryMock.Object, userManagerMock.Object, mapperMock.Object);
            var command = new CreateProfileCommand(profileDto);

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<AlreadyExists>().WithMessage("Profile with this name in your account already exists");
        }

        [Fact]
        public async Task Handler_Should_Create_Profile_When_Valid_Command_Is_Provided()
        {
            // Arrange
            var profileDto = new CreateProfileDTO
            {
                UserId = _faker.Random.Guid(),
                Name = _faker.Name.FullName(),
                Weight = _faker.Random.Double(50, 100),
                Height = _faker.Random.Double(150, 200),
                Birthday = DateOnly.FromDateTime(_faker.Date.Past(30, DateTime.Today.AddYears(-18))),
                Gender = Gender.Male,
                ActivityLevel = ActivityLevel.low
            };

            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            var profileRepositoryMock = new Mock<IProfileRepository>();
            profileRepositoryMock.Setup(repo => repo.GetAllByUserAsync((Guid)profileDto.UserId))
                .ReturnsAsync(new List<UserService.Domain.Entities.Profile>());

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<UserService.Domain.Entities.Profile>(profileDto))
                .Returns(new UserService.Domain.Entities.Profile { UserId = (Guid)profileDto.UserId, Name = profileDto.Name });

            var handler = new CreateProfileHandler(profileRepositoryMock.Object, userManagerMock.Object, mapperMock.Object);
            var command = new CreateProfileCommand(profileDto);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be((Guid)profileDto.UserId);
            result.Name.Should().Be(profileDto.Name);
            profileRepositoryMock.Verify(repo => repo.Add(It.IsAny<UserService.Domain.Entities.Profile>()), Times.Once);
            profileRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }

}
