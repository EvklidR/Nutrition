using AutoMapper;
using Bogus;
using FluentAssertions;
using MealPlanService.BusinessLogic.DTOs;
using MealPlanService.Core.Entities;
using MealPlanService.Core.Enums;
using MealPlanService.Infrastructure.Repositories.Interfaces;
using Moq;

namespace MealPlanServiceTests;

public class CreateMealPlanTests
{
    private readonly Mock<IMealPlanRepository> _mockMealPlanRepo;
    private readonly Mock<IProfileMealPlanRepository> _mockProfileMealPlanRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Faker<CreateMealPlanDTO> _mealPlanDTOfaker;
    private readonly Faker _faker;

    private readonly MealPlanService.BusinessLogic.Services.MealPlanService _service;

    public CreateMealPlanTests()
    {
        _mockMealPlanRepo = new Mock<IMealPlanRepository>();
        _mockProfileMealPlanRepo = new Mock<IProfileMealPlanRepository>();
        _mockMapper = new Mock<IMapper>();

        _service = new MealPlanService.BusinessLogic.Services.MealPlanService(
            _mockMealPlanRepo.Object,
            _mockProfileMealPlanRepo.Object,
            _mockMapper.Object);

        _faker = new Faker();
        _mealPlanDTOfaker = new Faker<CreateMealPlanDTO>()
            .RuleFor(mp => mp.Name, f => f.Commerce.ProductName())
            .RuleFor(mp => mp.Description, f => f.Lorem.Sentence())
            .RuleFor(mp => mp.Type, f => f.PickRandom<MealPlanType>());
    }

    [Fact]
    public async Task Should_CreateMealPlan_When_ValidDtoProvided()
    {
        // Arrange
        var mealPlanDto = _mealPlanDTOfaker.Generate();

        var mealPlan = new MealPlan()
        {
            Id = _faker.Random.AlphaNumeric(24),
            Name = mealPlanDto.Name,
            Description = mealPlanDto.Description,
            Type = mealPlanDto.Type,
        };

        _mockMapper.Setup(m => m.Map<MealPlan>(mealPlanDto)).Returns(mealPlan);
        _mockMealPlanRepo.Setup(r => r.CreateAsync(mealPlan)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateMealPlanAsync(mealPlanDto);

        // Assert
        result.Should().BeEquivalentTo(mealPlan);

        _mockMapper.Verify(m => m.Map<MealPlan>(mealPlanDto), Times.Once);
        _mockMealPlanRepo.Verify(r => r.CreateAsync(mealPlan), Times.Once);
    }
}
