using AutoMapper;
using Bogus;
using FluentAssertions;
using MealPlanService.Core.Enums;
using MealPlanService.Infrastructure.Projections;
using MealPlanService.Infrastructure.RabbitMQService;
using MealPlanService.Infrastructure.Repositories.Interfaces;
using Moq;

namespace MealPlanServiceTests;

public class GetMealPlansTests
{
    private readonly Mock<IMealPlanRepository> _mockMealPlanRepo;
    private readonly Mock<IProfileMealPlanRepository> _mockProfileMealPlanRepo;
    private readonly Mock<IBrokerService> _brokerService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly MealPlanService.BusinessLogic.Services.MealPlanService _service;

    private readonly Faker<MealPlanDTO> _mealPlanFaker;

    public GetMealPlansTests()
    {
        _mockMealPlanRepo = new Mock<IMealPlanRepository>();
        _mockProfileMealPlanRepo = new Mock<IProfileMealPlanRepository>();
        _mockMapper = new Mock<IMapper>();
        _brokerService = new Mock<IBrokerService>();

        _service = new MealPlanService.BusinessLogic.Services.MealPlanService(
            _mockMealPlanRepo.Object,
            _mockProfileMealPlanRepo.Object,
            _mockMapper.Object,
            _brokerService.Object);

        _mealPlanFaker = new Faker<MealPlanDTO>()
            .RuleFor(mp => mp.Id, f => f.Random.Guid().ToString())
            .RuleFor(mp => mp.Name, f => f.Commerce.ProductName())
            .RuleFor(mp => mp.Description, f => f.Lorem.Sentence())
            .RuleFor(mp => mp.Type, f => f.PickRandom<MealPlanType>());
    }

    [Fact]
    public async Task Should_ReturnMealPlansResponse_When_DataExists()
    {
        // Arrange
        var mealPlans = _mealPlanFaker.Generate(3);
        var totalCount = mealPlans.Count;

        _mockMealPlanRepo
            .Setup(r => r.GetAllAsync(null, null, null))
            .ReturnsAsync((mealPlans, totalCount));

        // Act
        var result = await _service.GetMealPlansAsync(null, null, null);

        // Assert
        result.Should().NotBeNull();
        result.MealPlans.Should().BeEquivalentTo(mealPlans);
        result.TotalCount.Should().Be(totalCount);

        _mockMealPlanRepo.Verify(r => r.GetAllAsync(null, null, null), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnEmptyResponse_When_NoMealPlansExist()
    {
        // Arrange
        var mealPlans = new List<MealPlanDTO>();
        var totalCount = 0;

        _mockMealPlanRepo
            .Setup(r => r.GetAllAsync(null, null, null))
            .ReturnsAsync((mealPlans, totalCount));

        // Act
        var result = await _service.GetMealPlansAsync(null, null, null);

        // Assert
        result.Should().NotBeNull();
        result.MealPlans.Should().BeEmpty();
        result.TotalCount.Should().Be(0);

        _mockMealPlanRepo.Verify(r => r.GetAllAsync(null, null, null), Times.Once);
    }

    [Fact]
    public async Task Should_FilterByType_When_TypeProvided()
    {
        // Arrange
        var filterType = MealPlanType.Maintenance;
        var mealPlans = _mealPlanFaker.Generate(10)
            .Select(mp => { mp.Type = filterType; return mp; })
            .ToList();
        var totalCount = mealPlans.Count;

        _mockMealPlanRepo
            .Setup(r => r.GetAllAsync(filterType, null, null))
            .ReturnsAsync((mealPlans, totalCount));

        // Act
        var result = await _service.GetMealPlansAsync(filterType, null, null);

        // Assert
        result.Should().NotBeNull();
        result.MealPlans.Should().BeEquivalentTo(mealPlans);
        result.TotalCount.Should().Be(totalCount);

        _mockMealPlanRepo.Verify(r => r.GetAllAsync(filterType, null, null), Times.Once);
    }
}
