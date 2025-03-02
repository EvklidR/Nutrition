using FluentAssertions;
using Moq;
using Bogus;
using MealPlanService.Core.Entities;
using MealPlanService.Infrastructure.Services.Interfaces;
using MealPlanService.Infrastructure.Repositories.Interfaces;
using MealPlanService.BusinessLogic.Services;
using AutoMapper;
using MealPlanService.BusinessLogic.Exceptions;
using MealPlanService.Infrastructure.RabbitMQService;

namespace MealPlanServiceTests
{
    public class GetProfilePlansTests
    {
        private readonly Mock<IProfileMealPlanRepository> _mockProfileMealPlanRepo;
        private readonly Mock<IMealPlanRepository> _mockMealPlanRepo;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IBrokerService> _brokerService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Faker _faker;

        private readonly Mock<MealPlanService.BusinessLogic.Services.MealPlanService> _mockMealPlanService;
        private readonly ProfilePlanService _profilePlanService;


        public GetProfilePlansTests()
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
        public async Task GetProfilePlans_Should_ThrowBadRequest_When_UserDoesNotHaveAccessToProfile()
        {
            // Arrange
            var userId = _faker.Random.Guid().ToString();
            var profileId = _faker.Random.Guid().ToString();

            _mockUserService.Setup(x => x.CheckProfileBelonging(userId, profileId))
                .ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _profilePlanService.GetProfilePlansAsync(userId, profileId);

            // Assert
            var exception = await act.Should().ThrowAsync<BadRequest>();
            exception.Which.Errors.Contains("You don't have access to this profile");
        }

        [Fact]
        public async Task GetProfilePlans_Should_ReturnMealPlans_When_UserHasAccessToProfile()
        {
            // Arrange
            var userId = _faker.Random.Guid().ToString();
            var profileId = _faker.Random.Guid().ToString();

            var mealPlans = new List<ProfileMealPlan>
            {
                new ProfileMealPlan { ProfileId = profileId, IsActive = true },
                new ProfileMealPlan { ProfileId = profileId, IsActive = false }
            };

            _mockUserService.Setup(x => x.CheckProfileBelonging(userId, profileId))
                .ReturnsAsync(true);

            _mockProfileMealPlanRepo.Setup(x => x.GetAllAsync(profileId))
                .ReturnsAsync(mealPlans);

            // Act
            var result = await _profilePlanService.GetProfilePlansAsync(userId, profileId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(mealPlans);
        }
    }
}
