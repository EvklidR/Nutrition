using FluentAssertions;
using Moq;
using Bogus;
using MealPlanService.Core.Entities;
using MealPlanService.Infrastructure.Services.Interfaces;
using MealPlanService.Infrastructure.Repositories.Interfaces;
using MealPlanService.BusinessLogic.Services;
using AutoMapper;
using MealPlanService.Infrastructure.RabbitMQService;

namespace MealPlanServiceTests
{
    public class DeleteProfilePlansTests
    {
        private readonly Mock<IProfileMealPlanRepository> _mockProfileMealPlanRepo;
        private readonly Mock<IMealPlanRepository> _mockMealPlanRepo;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IBrokerService> _brokerService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Faker _faker;

        private readonly Mock<MealPlanService.BusinessLogic.Services.MealPlanService> _mockMealPlanService;
        private readonly ProfilePlanService _profilePlanService;

        public DeleteProfilePlansTests()
        {
            _mockProfileMealPlanRepo = new Mock<IProfileMealPlanRepository>();
            _mockMealPlanRepo = new Mock<IMealPlanRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockUserService = new Mock<IUserService>();
            _brokerService = new Mock<IBrokerService>();
            _faker = new Faker();

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
        }

        [Fact]
        public async Task DeleteProfilePlans_Should_DeleteAllPlans_When_ProfileHasPlans()
        {
            // Arrange
            var profileId = _faker.Random.Guid().ToString();
            var mealPlans = new List<ProfileMealPlan>
            {
                new ProfileMealPlan { Id = _faker.Random.Guid().ToString(), ProfileId = profileId },
                new ProfileMealPlan { Id = _faker.Random.Guid().ToString(), ProfileId = profileId }
            };

            _mockProfileMealPlanRepo.Setup(x => x.GetAllAsync(profileId))
                .ReturnsAsync(mealPlans);

            // Act
            await _profilePlanService.DeleteProfilePlansAsync(profileId);

            // Assert
            _mockProfileMealPlanRepo.Verify(x => x.DeleteAsync(It.IsAny<string>()), Times.Exactly(mealPlans.Count));
        }

        [Fact]
        public async Task DeleteProfilePlans_Should_DoNothing_When_NoPlansExistForProfile()
        {
            // Arrange
            var profileId = _faker.Random.Guid().ToString();
            var mealPlans = new List<ProfileMealPlan>();

            _mockProfileMealPlanRepo.Setup(x => x.GetAllAsync(profileId))
                .ReturnsAsync(mealPlans);

            // Act
            await _profilePlanService.DeleteProfilePlansAsync(profileId);

            // Assert
            _mockProfileMealPlanRepo.Verify(x => x.DeleteAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
