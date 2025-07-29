using AutoMapper;
using Bogus;
using FluentAssertions;
using FoodService.Application.UseCases.Queries.Meal;
using FoodService.Application.UseCases.QueryHandlers.Meal.GetMealById;
using FoodService.Application.Interfaces;
using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces;
using Moq;
using FoodService.Application.Exceptions;

namespace FoodServiceTests.Meal
{
    public class GetMealByIdTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly IMapper _mapper;

        private readonly GetMealByIdHandler _handler;

        private readonly Faker _faker;

        public GetMealByIdTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userServiceMock = new Mock<IUserService>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FoodService.Application.Mappers.MealMappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _faker = new Faker();

            _handler = new GetMealByIdHandler(_mapper, _unitOfWorkMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnMeal_WhenMealExistsAndUserHasAccess()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var dayId = _faker.Random.Guid();
            var mealId = _faker.Random.Guid();

            var query = new GetMealByIdQuery(mealId, dayId, userId);

            var product1 = new FoodService.Domain.Entities.Product { Id = Guid.NewGuid(), Name = _faker.Random.Word() };
            var product2 = new FoodService.Domain.Entities.Product { Id = Guid.NewGuid(), Name = _faker.Random.Word() };

            var dish = new FoodService.Domain.Entities.Dish
            {
                Id = Guid.NewGuid(),
                Name = _faker.Random.Word()
            };

            var meal = new FoodService.Domain.Entities.Meal
            {
                Id = query.MealId,
                Name = _faker.Random.Word(),
                Dishes = new List<EatenDish> { new EatenDish { Food = dish} },
                Products = new List<EatenProduct> { new EatenProduct { Food = product1}, new EatenProduct { Food = product2 } }
            };

            var day = new FoodService.Domain.Entities.DayResult
            {
                Id = query.DayId,
                ProfileId = Guid.NewGuid(),
                Meals = new List<FoodService.Domain.Entities.Meal> { meal }
            };

            _unitOfWorkMock
                .Setup(u => u.DayResultRepository.GetByIdAsync(query.DayId))
                .ReturnsAsync(day);

            _userServiceMock
                .Setup(u => u.CheckProfileBelonging(userId, day.ProfileId))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(meal.Name);
            result.Dishes.Should().Contain(d => d.Name == dish.Name);
            result.Products.Should().Contain(p => p.Name == product1.Name);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenMealDoesNotExist()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var dayId = _faker.Random.Guid();
            var mealId = _faker.Random.Guid();

            var query = new GetMealByIdQuery(mealId, dayId, userId);

            var day = new FoodService.Domain.Entities.DayResult
            {
                Id = query.DayId,
                ProfileId = Guid.NewGuid(),
                Meals = new List<FoodService.Domain.Entities.Meal>()
            };

            _unitOfWorkMock
                .Setup(u => u.DayResultRepository.GetByIdAsync(query.DayId))
                .ReturnsAsync(day);

            _userServiceMock
                .Setup(u => u.CheckProfileBelonging(userId, day.ProfileId))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFound>()
                .WithMessage("Meal not found");
        }

        [Fact]
        public async Task Handle_ShouldThrowForbidden_WhenUserDoesNotHaveAccess()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var dayId = _faker.Random.Guid();
            var mealId = _faker.Random.Guid();

            var query = new GetMealByIdQuery(mealId, dayId, userId);

            var anotherUserId = Guid.NewGuid();

            var dish = new FoodService.Domain.Entities.Dish
            {
                Id = Guid.NewGuid(),
                Name = _faker.Random.Word()
            };

            var meal = new FoodService.Domain.Entities.Meal
            {
                Id = query.MealId,
                Name = _faker.Random.Word(),
                Dishes = new List<EatenDish> { new EatenDish { Food = dish } }
            };

            var day = new FoodService.Domain.Entities.DayResult
            {
                Id = query.DayId,
                ProfileId = Guid.NewGuid(),
                Meals = new List<FoodService.Domain.Entities.Meal> { meal }
            };

            _unitOfWorkMock
                .Setup(u => u.DayResultRepository.GetByIdAsync(query.DayId))
                .ReturnsAsync(day);

            _userServiceMock
                .Setup(u => u.CheckProfileBelonging(anotherUserId, day.ProfileId))
                .ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Forbidden>()
                .WithMessage("You dont have access to this meal");
        }
    }
}
