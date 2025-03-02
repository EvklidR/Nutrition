using AutoMapper;
using FluentAssertions;
using MealPlanService.BusinessLogic.DTOs;
using MealPlanService.BusinessLogic.Exceptions;
using MealPlanService.BusinessLogic.Services;
using MealPlanService.Core.Entities;
using MealPlanService.Infrastructure.Repositories.Interfaces;
using Moq;
using Bogus;
using MealPlanService.Infrastructure.Services.Interfaces;
using MealPlanService.Infrastructure.RabbitMQService;

namespace MealPlanServiceTests
{
    public class CreateProfilePlanTests
    {
        private readonly Mock<IProfileMealPlanRepository> _mockProfileMealPlanRepo;
        private readonly Mock<IMealPlanRepository> _mockMealPlanRepo;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IBrokerService> _brokerService;
        private readonly Mock<IMapper> _mockMapper;

        private readonly Mock<MealPlanService.BusinessLogic.Services.MealPlanService> _mockMealPlanService;
        private readonly ProfilePlanService _profilePlanService;

        private readonly Faker<ProfileMealPlanDTO> _profileMealPlanDTOFaker;
        private readonly Faker<ProfileMealPlan> _profileMealPlanFaker;

        public CreateProfilePlanTests()
        {
            _mockProfileMealPlanRepo = new Mock<IProfileMealPlanRepository>();
            _mockMealPlanRepo = new Mock<IMealPlanRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockUserService = new Mock<IUserService>();
            _brokerService = new Mock<IBrokerService>();
            
            _mockMealPlanService = new Mock<MealPlanService.BusinessLogic.Services.MealPlanService>(
                _mockMealPlanRepo.Object,
                _mockProfileMealPlanRepo.Object,
                _mockMapper.Object,
                _brokerService.Object);

            _profilePlanService = new ProfilePlanService(
                _mockProfileMealPlanRepo.Object,
                _mockMealPlanRepo.Object,
                _mockMealPlanService.Object,
                _mockUserService.Object,
                _brokerService.Object,
                _mockMapper.Object
            );

            _profileMealPlanDTOFaker = new Faker<ProfileMealPlanDTO>()
                .RuleFor(x => x.ProfileId, f => f.Random.Guid().ToString())
                .RuleFor(x => x.MealPlanId, f => f.Random.Guid().ToString());

            _profileMealPlanFaker = new Faker<ProfileMealPlan>()
                .RuleFor(x => x.ProfileId, f => f.Random.Guid().ToString())
                .RuleFor(x => x.IsActive, f => f.Random.Bool());
        }

        [Fact]
        public async Task CreateProfilePlanAsync_Should_ThrowBadRequest_When_UserDoesNotHaveAccessToProfile()
        {
            // Arrange
            var userId = new Faker().Random.Guid().ToString();
            var profileMealPlanDTO = _profileMealPlanDTOFaker.Generate();

            _mockUserService.Setup(x => x.CheckProfileBelonging(userId, profileMealPlanDTO.ProfileId))
                .ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _profilePlanService.CreateProfilePlanAsync(userId, profileMealPlanDTO);

            // Assert
            var errors = await act.Should().ThrowAsync<BadRequest>();
            errors.Which.Errors.Contains("You don't have access to this profile");
        }

        [Fact]
        public async Task CreateProfilePlanAsync_Should_ThrowNotFound_When_MealPlanNotFound()
        {
            // Arrange
            var userId = new Faker().Random.Guid().ToString();
            var profileMealPlanDTO = _profileMealPlanDTOFaker.Generate();

            _mockUserService.Setup(x => x.CheckProfileBelonging(userId, profileMealPlanDTO.ProfileId))
                .ReturnsAsync(true);
            _mockMealPlanRepo.Setup(x => x.GetByIdAsync(profileMealPlanDTO.MealPlanId))
                .ReturnsAsync((MealPlan)null);

            // Act
            Func<Task> act = async () => await _profilePlanService.CreateProfilePlanAsync(userId, profileMealPlanDTO);

            // Assert
            await act.Should().ThrowAsync<NotFound>()
                .WithMessage("Meal plan not found");
        }

        [Fact]
        public async Task CreateProfilePlanAsync_Should_UpdateActiveUserPlan_When_ActivePlanExists()
        {
            // Arrange
            var userId = new Faker().Random.Guid().ToString();
            var profileMealPlanDTO = _profileMealPlanDTOFaker.Generate();
            var existingActivePlan = _profileMealPlanFaker.Generate();
            existingActivePlan.ProfileId = profileMealPlanDTO.ProfileId;

            var newPlan = new MealPlan
            {
                Id = profileMealPlanDTO.MealPlanId
            };

            _mockUserService.Setup(x => x.CheckProfileBelonging(userId, profileMealPlanDTO.ProfileId))
                .ReturnsAsync(true);

            _mockMealPlanRepo.Setup(x => x.GetByIdAsync(profileMealPlanDTO.MealPlanId))
                .ReturnsAsync(newPlan);

            _mockProfileMealPlanRepo.Setup(x => x.GetActiveProfilePlan(profileMealPlanDTO.ProfileId))
                .ReturnsAsync(existingActivePlan);

            _mockProfileMealPlanRepo.Setup(x => x.UpdateAsync(It.IsAny<ProfileMealPlan>()))
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(x => x.Map<ProfileMealPlan>(profileMealPlanDTO))
                .Returns(new ProfileMealPlan());

            // Act
            var result = await _profilePlanService.CreateProfilePlanAsync(userId, profileMealPlanDTO);

            // Assert
            result.Should().NotBeNull();
            _mockProfileMealPlanRepo.Verify(x => x.UpdateAsync(It.IsAny<ProfileMealPlan>()), Times.Once);
        }

        [Fact]
        public async Task CreateProfilePlanAsync_Should_CreateNewProfileMealPlan_When_NoActivePlanExists()
        {
            // Arrange
            var userId = new Faker().Random.Guid().ToString();
            var profileMealPlanDTO = _profileMealPlanDTOFaker.Generate();

            var newPlan = new MealPlan
            {
                Id = profileMealPlanDTO.MealPlanId
            };

            _mockUserService.Setup(x => x.CheckProfileBelonging(userId, profileMealPlanDTO.ProfileId))
                .ReturnsAsync(true);

            _mockMealPlanRepo.Setup(x => x.GetByIdAsync(profileMealPlanDTO.MealPlanId))
                .ReturnsAsync(newPlan);

            _mockProfileMealPlanRepo.Setup(x => x.GetActiveProfilePlan(profileMealPlanDTO.ProfileId))
                .ReturnsAsync((ProfileMealPlan)null);

            _mockProfileMealPlanRepo.Setup(x => x.CreateAsync(It.IsAny<ProfileMealPlan>()))
                .Returns(Task.CompletedTask);

            _mockMapper.Setup(x => x.Map<ProfileMealPlan>(profileMealPlanDTO))
                .Returns(new ProfileMealPlan());

            // Act
            var result = await _profilePlanService.CreateProfilePlanAsync(userId, profileMealPlanDTO);

            // Assert
            result.Should().NotBeNull();
            _mockProfileMealPlanRepo.Verify(x => x.CreateAsync(It.IsAny<ProfileMealPlan>()), Times.Once);
        }
    }
}
