using FluentAssertions;
using Moq;
using Bogus;
using MealPlanService.Core.Entities;
using MealPlanService.Core.Enums;
using MealPlanService.BusinessLogic.Services;
using MealPlanService.BusinessLogic.Exceptions;
using MealPlanService.BusinessLogic.Models;
using AutoMapper;
using MealPlanService.Infrastructure.Repositories.Interfaces;
using MealPlanService.Infrastructure.Services.Interfaces;
using MealPlanService.Infrastructure.RabbitMQService;

namespace MealPlanServiceTests
{
    public class CalculateDailyNutrientsTests
    {
        private readonly Mock<IProfileMealPlanRepository> _mockProfileMealPlanRepo;
        private readonly Mock<IMealPlanRepository> _mockMealPlanRepo;
        private readonly Mock<MealPlanService.BusinessLogic.Services.MealPlanService> _mockMealPlanService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IBrokerService> _brokerService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProfilePlanService _profilePlanService;
        private readonly Faker _faker;

        public CalculateDailyNutrientsTests()
        {
            _mockProfileMealPlanRepo = new Mock<IProfileMealPlanRepository>();
            _mockMealPlanRepo = new Mock<IMealPlanRepository>();
            _mockMapper = new Mock<IMapper>();

            _mockUserService = new Mock<IUserService>();
            _brokerService = new Mock<IBrokerService>();

            _mockMealPlanService = new Mock<MealPlanService.BusinessLogic.Services.MealPlanService>(
                _mockMealPlanRepo.Object,
                _mockProfileMealPlanRepo.Object,
                _brokerService.Object,
                _mockMapper.Object);

            _profilePlanService = new ProfilePlanService(
                _mockProfileMealPlanRepo.Object,
                _mockMealPlanRepo.Object,
                _mockMealPlanService.Object,
                _mockUserService.Object,
                _brokerService.Object,
                _mockMapper.Object
            );

            _faker = new Faker();
        }

        [Fact]
        public async Task CalculateDailyNutrients_Should_CalculateCorrectly_When_CalculationTypeIsPerKg()
        {
            var profileId = _faker.Random.Guid().ToString();
            var request = new RequestForCalculating
            {
                ProfileId = profileId,
                BodyWeight = 70,
                DailyKcal = 2000
            };

            var day = new MealPlanDay
            {
                DayNumber = 1,
                CaloriePercentage = 0.5,
                NutrientsOfDay = new List<NutrientOfDay>
                {
                    new NutrientOfDay { NutrientType = NutrientType.Protein, CalculationType = CalculationType.PerKg, Value = 2.0 },
                    new NutrientOfDay { NutrientType = NutrientType.Fat, CalculationType = CalculationType.PerKg, Value = 1.5 },
                    new NutrientOfDay { NutrientType = NutrientType.Carbohydrate, CalculationType = CalculationType.PerKg, Value = 3.0 }
                }
            };

            _mockProfileMealPlanRepo.Setup(service => service.GetActiveProfilePlan(profileId))
                .ReturnsAsync(new ProfileMealPlan() 
                { 
                    MealPlanId = _faker.Random.AlphaNumeric(24),
                    StartDate = _faker.Date.PastDateOnly() 
                });

            _mockMealPlanRepo.Setup(service => service.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new MealPlan() { Days = [day] });

            var result = await _profilePlanService.CalculateDailyNutrientsAsync(request);

            result.Calories.Should().Be(1000);
            result.Proteins.Should().Be(140.0);
            result.Fats.Should().Be(105.0);
            result.Carbohydrates.Should().Be(210.0);
        }

        [Fact]
        public async Task CalculateDailyNutrients_Should_CalculateCorrectly_When_CalculationTypeIsPercent()
        {
            var profileId = _faker.Random.Guid().ToString();
            var request = new RequestForCalculating
            {
                ProfileId = profileId,
                BodyWeight = 70,
                DailyKcal = 2000
            };

            var day = new MealPlanDay
            {
                DayNumber = 1,
                CaloriePercentage = 0.5,
                NutrientsOfDay = new List<NutrientOfDay>
                {
                    new NutrientOfDay { NutrientType = NutrientType.Protein, CalculationType = CalculationType.Persent, Value = 0.25 },
                    new NutrientOfDay { NutrientType = NutrientType.Fat, CalculationType = CalculationType.Persent, Value = 0.2 },
                    new NutrientOfDay { NutrientType = NutrientType.Carbohydrate, CalculationType = CalculationType.Persent, Value = 0.55 }
                }
            };

            _mockProfileMealPlanRepo.Setup(service => service.GetActiveProfilePlan(profileId))
                .ReturnsAsync(new ProfileMealPlan()
                {
                    MealPlanId = _faker.Random.AlphaNumeric(24),
                    StartDate = _faker.Date.PastDateOnly()
                });

            _mockMealPlanRepo.Setup(service => service.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new MealPlan() { Days = [day] });

            var result = await _profilePlanService.CalculateDailyNutrientsAsync(request);

            result.Calories.Should().Be(1000);
            result.Proteins.Should().Be(62.5);
            result.Fats.Should().Be(22.22222222222222);
            result.Carbohydrates.Should().Be(137.5);
        }

        [Fact]
        public async Task CalculateDailyNutrients_Should_CalculateCorrectly_When_CalculationTypeIsFixed()
        {
            var profileId = _faker.Random.Guid().ToString();
            var request = new RequestForCalculating
            {
                ProfileId = profileId,
                BodyWeight = 70,
                DailyKcal = 2000
            };

            var day = new MealPlanDay
            {
                DayNumber = 1,
                CaloriePercentage = 0.5,
                NutrientsOfDay = new List<NutrientOfDay>
                {
                    new NutrientOfDay { NutrientType = NutrientType.Protein, CalculationType = CalculationType.Fixed, Value = 100 },
                    new NutrientOfDay { NutrientType = NutrientType.Fat, CalculationType = CalculationType.Fixed, Value = 70 },
                    new NutrientOfDay { NutrientType = NutrientType.Carbohydrate, CalculationType = CalculationType.Fixed, Value = 200 }
                }
            };

            _mockProfileMealPlanRepo.Setup(service => service.GetActiveProfilePlan(profileId))
                .ReturnsAsync(new ProfileMealPlan()
                {
                    MealPlanId = _faker.Random.AlphaNumeric(24),
                    StartDate = _faker.Date.PastDateOnly()
                });

            _mockMealPlanRepo.Setup(service => service.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new MealPlan() { Days = [day] });

            var result = await _profilePlanService.CalculateDailyNutrientsAsync(request);

            result.Calories.Should().Be(1000);
            result.Proteins.Should().Be(100);
            result.Fats.Should().Be(70);
            result.Carbohydrates.Should().Be(200);
        }

        [Fact]
        public async Task CalculateDailyNutrients_Should_ThrowException_When_NutrientsDataIsMissing()
        {
            var profileId = _faker.Random.Guid().ToString();
            var request = new RequestForCalculating
            {
                ProfileId = profileId,
                BodyWeight = 70,
                DailyKcal = 2000
            };

            _mockProfileMealPlanRepo.Setup(service => service.GetActiveProfilePlan(profileId))
                .ReturnsAsync(new ProfileMealPlan()
                {
                    MealPlanId = _faker.Random.AlphaNumeric(24),
                    StartDate = _faker.Date.PastDateOnly()
                });

            _mockMealPlanRepo.Setup(service => service.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new MealPlan() { Days = [new MealPlanDay() ]});

            Func<Task> act = async () => await _profilePlanService.CalculateDailyNutrientsAsync(request);

            await act.Should().ThrowAsync<NotFound>().WithMessage("Meal plan day not found");
        }
    }
}
