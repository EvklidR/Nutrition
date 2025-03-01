using AutoMapper;
using Bogus;
using FluentAssertions;
using MealPlanService.BusinessLogic.Exceptions;
using MealPlanService.Core.Entities;
using MealPlanService.Infrastructure.RabbitMQService;
using MealPlanService.Infrastructure.Repositories.Interfaces;
using MongoDB.Driver;
using Moq;

namespace MealPlanServiceTests
{
    public class DeleteMealPlanTests
    {
        private readonly Mock<IMealPlanRepository> _mockMealPlanRepo;
        private readonly Mock<IProfileMealPlanRepository> _mockProfileMealPlanRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Faker<MealPlan> _mealPlanFaker;
        private readonly Faker<ProfileMealPlan> _profileMealPlanFaker;
        private readonly Mock<IBrokerService> _brokerService;

        private readonly MealPlanService.BusinessLogic.Services.MealPlanService _service;

        public DeleteMealPlanTests()
        {
            _mockMealPlanRepo = new Mock<IMealPlanRepository>();
            _mockProfileMealPlanRepo = new Mock<IProfileMealPlanRepository>();
            _brokerService = new Mock<IBrokerService>();
            _mockMapper = new Mock<IMapper>();

            _service = new MealPlanService.BusinessLogic.Services.MealPlanService(
                _mockMealPlanRepo.Object,
                _mockProfileMealPlanRepo.Object,
                _mockMapper.Object,
                _brokerService.Object);

            _mealPlanFaker = new Faker<MealPlan>()
                .RuleFor(m => m.Id, f => f.Random.AlphaNumeric(1))
                .RuleFor(m => m.Name, f => f.Commerce.ProductName());

            _profileMealPlanFaker = new Faker<ProfileMealPlan>()
                .RuleFor(p => p.Id, f => f.Random.AlphaNumeric(1))
                .RuleFor(p => p.ProfileId, f => f.Random.Guid().ToString())
                .RuleFor(p => p.MealPlanId, f => f.Random.AlphaNumeric(1));
        }

        [Fact]
        public async Task Should_DeleteMealPlanAndRelatedProfilePlans_When_MealPlanExists()
        {
            // Arrange
            var mealPlan = _mealPlanFaker.Generate();
            var profilePlans = _profileMealPlanFaker.Generate(20).Select(p =>
            {
                p.MealPlanId = mealPlan.Id;
                return p;
            }).ToList();

            _mockMealPlanRepo.Setup(r => r.GetByIdAsync(mealPlan.Id)).ReturnsAsync(mealPlan);
            _mockMealPlanRepo.Setup(r => r.DeleteAsync(mealPlan.Id)).Returns(Task.CompletedTask);
            _mockProfileMealPlanRepo.Setup(r => r.GetByMealPlan(mealPlan.Id)).ReturnsAsync(profilePlans);
            _mockProfileMealPlanRepo.Setup(r => r.DeleteAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteMealPlanAsync(mealPlan.Id);

            // Assert
            _mockMealPlanRepo.Verify(r => r.GetByIdAsync(mealPlan.Id), Times.Once);
            _mockMealPlanRepo.Verify(r => r.DeleteAsync(mealPlan.Id), Times.Once);
            _mockProfileMealPlanRepo.Verify(r => r.GetByMealPlan(mealPlan.Id), Times.Once);
            _mockProfileMealPlanRepo.Verify(r => r.DeleteAsync(It.IsAny<string>()), Times.Exactly(profilePlans.Count));
        }

        [Fact]
        public async Task Should_ThrowNotFound_When_MealPlanDoesntExists()
        {
            // Arrange
            var mealPlan = _profileMealPlanFaker.Generate();

            _mockMealPlanRepo.Setup(r => r.GetByIdAsync(mealPlan.Id)).ReturnsAsync((MealPlan)null);

            // Act
            Func<Task> act = async () => await _service.DeleteMealPlanAsync(mealPlan.Id);

            // Assert
            await act.Should().ThrowAsync<NotFound>().WithMessage("Meal plan not found");

            _mockMealPlanRepo.Verify(r => r.GetByIdAsync(mealPlan.Id), Times.Once);
            _mockMealPlanRepo.Verify(r => r.DeleteAsync(mealPlan.Id), Times.Never);
            _mockProfileMealPlanRepo.Verify(r => r.GetByMealPlan(mealPlan.Id), Times.Never);
            
        }
    }

}
