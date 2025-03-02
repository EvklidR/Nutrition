using AutoMapper;
using Moq;
using MealPlanService.BusinessLogic.Services;
using MealPlanService.Core.Entities;
using MealPlanService.Infrastructure.Repositories.Interfaces;
using FluentAssertions;
using Bogus;
using MealPlanService.Infrastructure.Services.Interfaces;
using MealPlanService.Infrastructure.RabbitMQService;

namespace MealPlanServiceTests
{
    public class GetRecommendationsTests
    {
        private readonly Mock<IProfileMealPlanRepository> _mockProfileMealPlanRepo;
        private readonly Mock<IMealPlanRepository> _mockMealPlanRepo;
        private readonly Mock<MealPlanService.BusinessLogic.Services.MealPlanService> _mockMealPlanService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IBrokerService> _brokerService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProfilePlanService _profilePlanService;
        private readonly Faker<Recommendation> _RecommendationsFaker;
        private readonly Faker _faker;

        public GetRecommendationsTests()
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

            _faker = new Faker();
            _RecommendationsFaker = new Faker<Recommendation>()
                .RuleFor(r => r.Text, f => f.Lorem.Paragraph());
        }

        [Fact]
        public async Task GetRecommendations_Should_ReturnRecommendations_When_DayHasRecommendations()
        {
            // Arrange
            var profileId = _faker.Random.Guid().ToString();
            var recommendations = _RecommendationsFaker.Generate(2);
            var startDate = _faker.Date.PastDateOnly();

            var currentDay = new MealPlanDay
            {
                DayNumber = 1,
                Recommendations = recommendations
            };

            _mockProfileMealPlanRepo.Setup(service => service.GetActiveProfilePlan(profileId))
                .ReturnsAsync(new ProfileMealPlan() { MealPlanId = _faker.Random.AlphaNumeric(24), StartDate = startDate });

            _mockMealPlanRepo.Setup(service => service.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new MealPlan() { Days = [currentDay] });


            // Act
            var result = await _profilePlanService.GetRecommendations(profileId);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(recommendations.Count);

            _mockProfileMealPlanRepo.Verify(service => service.GetActiveProfilePlan(profileId), Times.Once);
            _mockMealPlanRepo.Verify(service => service.GetByIdAsync(It.IsAny<string>()), Times.Once);
        }
    }
}
