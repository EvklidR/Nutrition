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
        private readonly Faker<CreateProfileDTO> _createProfileDTOFaker;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IProfileRepository> _profileRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly CreateProfileHandler _handler;

        public CreateProfileTests()
        {
            _createProfileDTOFaker = new Faker<CreateProfileDTO>()
                .RuleFor(p => p.UserId, f => f.Random.Guid())
                .RuleFor(p => p.Name, f => f.Name.FullName())
                .RuleFor(p => p.Weight, f => f.Random.Double(50, 100))
                .RuleFor(p => p.Height, f => f.Random.Double(150, 200))
                .RuleFor(p => p.Birthday, f => DateOnly.FromDateTime(f.Date.Past(30, DateTime.Today.AddYears(-18))))
                .RuleFor(p => p.Gender, f => f.Random.Enum<Gender>())
                .RuleFor(p => p.ActivityLevel, f => f.Random.Enum<ActivityLevel>());


            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _profileRepositoryMock = new Mock<IProfileRepository>();
            _mapperMock = new Mock<IMapper>();

            _handler = new CreateProfileHandler(_profileRepositoryMock.Object, _userManagerMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handler_Should_Throw_Unauthorized_When_User_Does_Not_Exist()
        {
            // Arrange
            var profileDto = _createProfileDTOFaker.Generate();

            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            _mapperMock.Setup(m => m.Map<UserService.Domain.Entities.Profile>(profileDto))
                .Returns(new UserService.Domain.Entities.Profile { UserId = (Guid)profileDto.UserId });

            var command = new CreateProfileCommand(profileDto);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Unauthorized>().WithMessage("User does not exist");
        }

        [Fact]
        public async Task Handler_Should_Throw_AlreadyExists_When_Profile_With_Same_Name_Exists()
        {
            // Arrange
            var profileDto = _createProfileDTOFaker.Generate();

            var existingProfiles = new List<UserService.Domain.Entities.Profile>
                {
                    new UserService.Domain.Entities.Profile { UserId = (Guid)profileDto.UserId, Name = profileDto.Name }
                };

            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            _profileRepositoryMock.Setup(repo => repo.GetAllByUserAsync((Guid)profileDto.UserId))
                .ReturnsAsync(existingProfiles);

            _mapperMock.Setup(m => m.Map<UserService.Domain.Entities.Profile>(profileDto))
                .Returns(new UserService.Domain.Entities.Profile { UserId = (Guid)profileDto.UserId, Name = profileDto.Name });

            var command = new CreateProfileCommand(profileDto);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<AlreadyExists>().WithMessage("Profile with this name in your account already exists");
        }

        [Fact]
        public async Task Handler_Should_Create_Profile_When_Valid_Command_Is_Provided()
        {
            // Arrange
            var profileDto = _createProfileDTOFaker.Generate();

            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new User());

            _profileRepositoryMock.Setup(repo => repo.GetAllByUserAsync((Guid)profileDto.UserId))
                .ReturnsAsync(new List<UserService.Domain.Entities.Profile>());

            _mapperMock.Setup(m => m.Map<UserService.Domain.Entities.Profile>(profileDto))
                .Returns(new UserService.Domain.Entities.Profile { UserId = (Guid)profileDto.UserId, Name = profileDto.Name });

            var command = new CreateProfileCommand(profileDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be((Guid)profileDto.UserId);
            result.Name.Should().Be(profileDto.Name);
            _profileRepositoryMock.Verify(repo => repo.Add(It.IsAny<UserService.Domain.Entities.Profile>()), Times.Once);
            _profileRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}
