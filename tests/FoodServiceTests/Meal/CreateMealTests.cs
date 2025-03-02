using AutoMapper;
using Bogus;
using FluentAssertions;
using FoodService.Application.DTOs.Meal;
using FoodService.Application.UseCases.Commands.Meal;
using FoodService.Application.UseCases.CommandHandlers.Meal;
using FoodService.Application.Interfaces;
using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces;
using Moq;
using FoodService.Application.Exceptions;

namespace FoodServiceTests.Meal
{
    public class CreateMealTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly IMapper _mapper;

        private readonly CreateMealHandler _handler;

        private readonly Faker<CreateMealDTO> _createMealDtoFaker;
        private readonly Faker<FoodService.Domain.Entities.Product> _productFaker;
        private readonly Faker<FoodService.Domain.Entities.Dish> _dishFaker;

        public CreateMealTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userServiceMock = new Mock<IUserService>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FoodService.Application.Mappers.MealMappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _createMealDtoFaker = new Faker<CreateMealDTO>()
                .RuleFor(m => m.DayId, f => f.Random.Guid())
                .RuleFor(m => m.Products, f => new List<CreateOrUpdateEatenFoodDTO>
                {
                    new CreateOrUpdateEatenFoodDTO { FoodId = f.Random.Guid(), Weight = 100 },
                    new CreateOrUpdateEatenFoodDTO { FoodId = f.Random.Guid(), Weight = 200 }
                })
                .RuleFor(m => m.Dishes, f => new List<CreateOrUpdateEatenFoodDTO>
                {
                    new CreateOrUpdateEatenFoodDTO { FoodId = f.Random.Guid(), Weight = 150 }
                });

            _productFaker = new Faker<FoodService.Domain.Entities.Product>()
                .RuleFor(p => p.Calories, f => f.Random.Int(5, 100))
                .RuleFor(p => p.Proteins, f => f.Random.Int(5, 100))
                .RuleFor(p => p.Fats, f => f.Random.Int(5, 100))
                .RuleFor(p => p.Carbohydrates, f => f.Random.Int(5, 100));

            _dishFaker = new Faker<FoodService.Domain.Entities.Dish>()
                .RuleFor(p => p.Calories, f => f.Random.Int(5, 100))
                .RuleFor(p => p.Proteins, f => f.Random.Int(5, 100))
                .RuleFor(p => p.Fats, f => f.Random.Int(5, 100))
                .RuleFor(p => p.Carbohydrates, f => f.Random.Int(5, 100));

            _handler = new CreateMealHandler(
                _unitOfWorkMock.Object,
                _mapper,
                _userServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateMealSuccessfully()
        {
            // Arrange
            var createMealDto = _createMealDtoFaker.Generate();

            var command = new CreateMealCommand(createMealDto, Guid.NewGuid());

            var day = new FoodService.Domain.Entities.DayResult
            {
                Id = createMealDto.DayId,
                ProfileId = Guid.NewGuid()
            };

            var products = _productFaker.Generate(2);
            products[0].Id = createMealDto.Products[0].FoodId;
            products[1].Id = createMealDto.Products[1].FoodId;

            var dish = _dishFaker.Generate();
            dish.Id = createMealDto.Dishes[0].FoodId;

            _unitOfWorkMock.Setup(u => u.DayResultRepository.GetByIdAsync(createMealDto.DayId))
                .ReturnsAsync(day);

            _userServiceMock.Setup(u => u.CheckProfileBelonging(It.IsAny<Guid>(), day.ProfileId))
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetByIdAsync(createMealDto.Products[0].FoodId))
                .ReturnsAsync(products[0]);
            _unitOfWorkMock.Setup(u => u.ProductRepository.GetByIdAsync(createMealDto.Products[1].FoodId))
                .ReturnsAsync(products[1]);

            _unitOfWorkMock.Setup(u => u.DishRepository.GetByIdAsync(createMealDto.Dishes[0].FoodId))
                .ReturnsAsync(dish);


            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Products.Should().HaveCount(2);
            result.Dishes.Should().HaveCount(1);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenDayDoesNotExist()
        {
            // Arrange
            var createMealDto = _createMealDtoFaker.Generate();
            var command = new CreateMealCommand(createMealDto, Guid.NewGuid());

            _unitOfWorkMock.Setup(u => u.DayResultRepository.GetByIdAsync(createMealDto.DayId))
                .ReturnsAsync((FoodService.Domain.Entities.DayResult)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFound>().WithMessage("Day not found");
        }

        [Fact]
        public async Task Handle_ShouldThrowForbidden_WhenUserHasNoAccess()
        {
            // Arrange
            var createMealDto = _createMealDtoFaker.Generate();
            var command = new CreateMealCommand(createMealDto, Guid.NewGuid());
            var day = new FoodService.Domain.Entities.DayResult { 
                Id = createMealDto.DayId, 
                ProfileId = Guid.NewGuid(),
                Meals = new List<FoodService.Domain.Entities.Meal>() };

            _unitOfWorkMock.Setup(u => u.DayResultRepository.GetByIdAsync(createMealDto.DayId))
                .ReturnsAsync(day);

            _userServiceMock.Setup(u => u.CheckProfileBelonging(It.IsAny<Guid>(), day.ProfileId))
                .ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Forbidden>().WithMessage("You dont have access to this meal");
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var createMealDto = _createMealDtoFaker.Generate();
            var command = new CreateMealCommand(createMealDto, Guid.NewGuid());
            var day = new FoodService.Domain.Entities.DayResult { 
                Id = createMealDto.DayId, 
                ProfileId = Guid.NewGuid(), 
                Meals = new List<FoodService.Domain.Entities.Meal>() };

            _unitOfWorkMock.Setup(u => u.DayResultRepository.GetByIdAsync(createMealDto.DayId))
                .ReturnsAsync(day);

            _userServiceMock.Setup(u => u.CheckProfileBelonging(It.IsAny<Guid>(), day.ProfileId))
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetByIdAsync(createMealDto.Products[0].FoodId))
                .ReturnsAsync((FoodService.Domain.Entities.Product)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFound>().WithMessage("Product not found");
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenDishDoesNotExist()
        {
            // Arrange
            var createMealDto = _createMealDtoFaker.Generate();

            var command = new CreateMealCommand(createMealDto, Guid.NewGuid());

            var day = new FoodService.Domain.Entities.DayResult { Id = createMealDto.DayId, ProfileId = Guid.NewGuid(), Meals = new List<FoodService.Domain.Entities.Meal>() };

            var product1 = new FoodService.Domain.Entities.Product { Id = createMealDto.Products[0].FoodId };
            var product2 = new FoodService.Domain.Entities.Product { Id = createMealDto.Products[1].FoodId };

            _unitOfWorkMock.Setup(u => u.DayResultRepository.GetByIdAsync(createMealDto.DayId))
                .ReturnsAsync(day);

            _userServiceMock.Setup(u => u.CheckProfileBelonging(It.IsAny<Guid>(), day.ProfileId))
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetByIdAsync(createMealDto.Products[0].FoodId))
                .ReturnsAsync(product1);

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetByIdAsync(createMealDto.Products[1].FoodId))
                .ReturnsAsync(product2);

            _unitOfWorkMock.Setup(u => u.DishRepository.GetByIdAsync(createMealDto.Dishes[0].FoodId))
                .ReturnsAsync((FoodService.Domain.Entities.Dish)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFound>().WithMessage("Dish not found");
        }
    }
}
