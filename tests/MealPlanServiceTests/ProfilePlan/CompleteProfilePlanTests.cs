using FluentAssertions;
using Moq;
using Bogus;
using MealPlanService.Core.Entities;
using MealPlanService.Infrastructure.Services.Interfaces;
using MealPlanService.Infrastructure.Repositories.Interfaces;
using MealPlanService.BusinessLogic.Services;
using MealPlanService.BusinessLogic.Exceptions;
using AutoMapper;

namespace MealPlanServiceTests
{
    public class CompleteProfilePlanTests
    {
        private readonly Mock<IProfileMealPlanRepository> _mockProfileMealPlanRepo;
        private readonly Mock<IMealPlanRepository> _mockMealPlanRepo;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Faker _faker;

        private readonly Mock<MealPlanService.BusinessLogic.Services.MealPlanService> _mockMealPlanService;
        private readonly ProfilePlanService _profilePlanService;


        public CompleteProfilePlanTests()
        {
            _mockProfileMealPlanRepo = new Mock<IProfileMealPlanRepository>();
            _mockMealPlanRepo = new Mock<IMealPlanRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockUserService = new Mock<IUserService>();
            _faker = new Faker();

            _mockMealPlanService = new Mock<MealPlanService.BusinessLogic.Services.MealPlanService>(
                _mockMealPlanRepo.Object,
                _mockProfileMealPlanRepo.Object,
                _mockMapper.Object);

            _profilePlanService = new ProfilePlanService(
                _mockProfileMealPlanRepo.Object,
                _mockMealPlanRepo.Object,
                _mockMealPlanService.Object,
                _mockUserService.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task CompleteProfilePlan_Should_ThrowBadRequest_When_UserDoesNotHaveAccessToProfile()
        {
            // Arrange
            var userId = _faker.Random.Guid().ToString();
            var profileId = _faker.Random.Guid().ToString();

            _mockUserService.Setup(x => x.CheckProfileBelonging(userId, profileId))
                .ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _profilePlanService.CompleteProfilePlanAsync(userId, profileId);

            // Assert
            var exception = await act.Should().ThrowAsync<BadRequest>();
            exception.Which.Errors.Contains("You don't have access to this profile");
        }

        [Fact]
        public async Task CompleteProfilePlan_Should_ThrowNotFound_When_ActivePlanNotFound()
        {
            // Arrange
            var userId = _faker.Random.Guid().ToString();
            var profileId = _faker.Random.Guid().ToString();

            _mockUserService.Setup(x => x.CheckProfileBelonging(userId, profileId))
                .ReturnsAsync(true);

            _mockProfileMealPlanRepo.Setup(x => x.GetActiveProfilePlan(profileId))
                .ReturnsAsync((ProfileMealPlan)null);

            // Act
            Func<Task> act = async () => await _profilePlanService.CompleteProfilePlanAsync(userId, profileId);

            // Assert
            await act.Should().ThrowAsync<NotFound>().WithMessage("Active plan not found");
        }

        [Fact]
        public async Task CompleteProfilePlan_Should_UpdateAndCompletePlan_When_ActivePlanExists()
        {
            // Arrange
            var userId = _faker.Random.Guid().ToString();
            var profileId = _faker.Random.Guid().ToString();

            var existingPlan = new ProfileMealPlan
            {
                Id = _faker.Random.Guid().ToString(),
                ProfileId = profileId,
                IsActive = true
            };

            _mockUserService.Setup(x => x.CheckProfileBelonging(userId, profileId))
                .ReturnsAsync(true);

            _mockProfileMealPlanRepo.Setup(x => x.GetActiveProfilePlan(profileId))
                .ReturnsAsync(existingPlan);

            // Act
            await _profilePlanService.CompleteProfilePlanAsync(userId, profileId);

            // Assert
            _mockProfileMealPlanRepo.Verify(x => x.UpdateAsync(It.Is<ProfileMealPlan>(p => p.IsActive == false && p.EndDate != DateOnly.MinValue)), Times.Once); // Проверяем, что обновление плана произошло
        }

        [Fact]
        public async Task CompleteProfilePlan_Should_ThrowException_When_UpdateFails()
        {
            // Arrange
            var userId = _faker.Random.Guid().ToString();
            var profileId = _faker.Random.Guid().ToString();

            var existingPlan = new ProfileMealPlan
            {
                Id = _faker.Random.Guid().ToString(),
                ProfileId = profileId,
                IsActive = true
            };

            _mockUserService.Setup(x => x.CheckProfileBelonging(userId, profileId))
                .ReturnsAsync(true);

            _mockProfileMealPlanRepo.Setup(x => x.GetActiveProfilePlan(profileId))
                .ReturnsAsync(existingPlan);

            _mockProfileMealPlanRepo.Setup(x => x.UpdateAsync(It.IsAny<ProfileMealPlan>()))
                .ThrowsAsync(new Exception("Update failed"));

            // Act
            Func<Task> act = async () => await _profilePlanService.CompleteProfilePlanAsync(userId, profileId);

            // Assert
            var exception = await act.Should().ThrowAsync<Exception>();
            exception.Which.Message.Should().Be("Update failed");
        }
    }
}
