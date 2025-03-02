using AutoMapper;
using Bogus;
using FluentAssertions;
using MealPlanService.BusinessLogic.Exceptions;
using MealPlanService.Core.Entities;
using MealPlanService.Core.Enums;
using MealPlanService.Infrastructure.RabbitMQService;
using MealPlanService.Infrastructure.Repositories.Interfaces;
using Moq;

namespace MealPlanServiceTests
{
    public class UpdateMealPlanTests
    {
        private readonly Mock<IMealPlanRepository> _mockMealPlanRepo;
        private readonly Mock<IProfileMealPlanRepository> _mockProfileMealPlanRepo;
        private readonly Mock<IBrokerService> _brokerService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Faker<MealPlan> _mealPlanFaker;
        private readonly Faker _faker;
        private readonly MealPlanService.BusinessLogic.Services.MealPlanService _service;

        public UpdateMealPlanTests() 
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

            _faker = new Faker();
            _mealPlanFaker = new Faker<MealPlan>()
                .RuleFor(mp => mp.Name, f => f.Commerce.ProductName())
                .RuleFor(mp => mp.Description, f => f.Lorem.Sentence())
                .RuleFor(mp => mp.Type, f => f.PickRandom<MealPlanType>());
        }

        [Fact]
        public async Task Should_UpdateMealPlan_When_MealPlanExists()
        {
            // Arrange
            var commonId = _faker.Random.AlphaNumeric(24);

            var updatedMealPlan = _mealPlanFaker.Generate();
            updatedMealPlan.Id = commonId;
            var existingMealPlan = _mealPlanFaker.Generate();
            existingMealPlan.Id = commonId;

            _mockMealPlanRepo.Setup(r => r.GetByIdAsync(updatedMealPlan.Id)).ReturnsAsync(existingMealPlan);
            _mockMealPlanRepo.Setup(r => r.UpdateAsync(updatedMealPlan)).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateMealPlanAsync(updatedMealPlan);

            // Assert
            _mockMealPlanRepo.Verify(r => r.GetByIdAsync(updatedMealPlan.Id), Times.Once);
            _mockMealPlanRepo.Verify(r => r.UpdateAsync(updatedMealPlan), Times.Once);
        }

        [Fact]
        public async Task Should_ThrowNotFound_When_MealPlanDoesNotExist()
        {
            // Arrange
            var updatedMealPlan = _mealPlanFaker.Generate();

            _mockMealPlanRepo.Setup(r => r.GetByIdAsync(updatedMealPlan.Id)).ReturnsAsync((MealPlan)null);

            // Act
            var act = async () => await _service.UpdateMealPlanAsync(updatedMealPlan);

            // Assert
            await act.Should().ThrowAsync<NotFound>()
                .WithMessage("Meal plan not found");

            _mockMealPlanRepo.Verify(r => r.GetByIdAsync(updatedMealPlan.Id), Times.Once);
            _mockMealPlanRepo.Verify(r => r.UpdateAsync(It.IsAny<MealPlan>()), Times.Never);
        }
    }
}
