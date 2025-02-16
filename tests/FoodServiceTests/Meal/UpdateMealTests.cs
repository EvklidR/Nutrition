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
    public class UpdateMealTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly IMapper _mapper;

        private readonly UpdateMealHandler _handler;

        private readonly Faker<UpdateMealDTO> _updateMealDtoFaker;

        public UpdateMealTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userServiceMock = new Mock<IUserService>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FoodService.Application.Mappers.MealMappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _updateMealDtoFaker = new Faker<UpdateMealDTO>()
                .RuleFor(m => m.Id, f => f.Random.Guid())
                .RuleFor(m => m.DayId, f => f.Random.Guid())
                .RuleFor(m => m.Dishes, f => new List<CreateOrUpdateEatenFoodDTO>
                {
                    new CreateOrUpdateEatenFoodDTO { FoodId = f.Random.Guid(), Weight = 300 }
                })
                .RuleFor(m => m.Products, f => new List<CreateOrUpdateEatenFoodDTO>
                {
                    new CreateOrUpdateEatenFoodDTO { FoodId = f.Random.Guid(), Weight = 300 },
                    new CreateOrUpdateEatenFoodDTO { FoodId = f.Random.Guid(), Weight = 150 }
                });

            _handler = new UpdateMealHandler(_unitOfWorkMock.Object, _mapper, _userServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateMeal_WhenMealExistsAndUserHasAccess()
        {
            // Arrange
            var updateMealDto = _updateMealDtoFaker.Generate();
            var userId = Guid.NewGuid();
            var command = new UpdateMealCommand(updateMealDto, userId);

            var product1 = new FoodService.Domain.Entities.Product { Id = updateMealDto.Products[0].FoodId, Calories = 120, Proteins = 10, Fats = 5, Carbohydrates = 20 };
            var product2 = new FoodService.Domain.Entities.Product { Id = updateMealDto.Products[1].FoodId, Calories = 200, Proteins = 15, Fats = 10, Carbohydrates = 30 };

            var dish = new FoodService.Domain.Entities.Dish
            {
                Id = updateMealDto.Dishes[0].FoodId,
                Name = "Old Dish",
                ImageUrl = "old_image_url"
            };

            var day = new FoodService.Domain.Entities.DayResult
            {
                Id = updateMealDto.DayId,
                ProfileId = Guid.NewGuid(),
                Meals = new List<FoodService.Domain.Entities.Meal>
                {
                    new FoodService.Domain.Entities.Meal { 
                        Id = updateMealDto.Id, 
                        Name = "Old Meal"
                    }
                }
            };

            _unitOfWorkMock
                .Setup(u => u.DayResultRepository.GetByIdAsync(updateMealDto.DayId))
                .ReturnsAsync(day);

            _userServiceMock
                .Setup(u => u.CheckProfileBelonging(userId, day.ProfileId))
                .ReturnsAsync(true);

            _unitOfWorkMock
                .Setup(u => u.ProductRepository.GetByIdAsync(updateMealDto.Products[0].FoodId))
                .ReturnsAsync(product1);

            _unitOfWorkMock
                .Setup(u => u.ProductRepository.GetByIdAsync(updateMealDto.Products[1].FoodId))
                .ReturnsAsync(product2);

            _unitOfWorkMock
                .Setup(u => u.DishRepository.GetByIdAsync(updateMealDto.Dishes[0].FoodId))
                .ReturnsAsync(dish);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenMealNotFound()
        {
            // Arrange
            var updateMealDto = _updateMealDtoFaker.Generate();
            var userId = Guid.NewGuid();
            var command = new UpdateMealCommand(updateMealDto, userId);

            var day = new FoodService.Domain.Entities.DayResult
            {
                Id = updateMealDto.DayId,
                ProfileId = Guid.NewGuid(),
                Meals = new List<FoodService.Domain.Entities.Meal>()
            };

            _unitOfWorkMock
                .Setup(u => u.DayResultRepository.GetByIdAsync(updateMealDto.DayId))
                .ReturnsAsync(day);

            _userServiceMock
                .Setup(u => u.CheckProfileBelonging(userId, day.ProfileId))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFound>()
                .WithMessage("Meal not found");

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowForbidden_WhenUserDoesNotHaveAccess()
        {
            // Arrange
            var updateMealDto = _updateMealDtoFaker.Generate();
            var ownerId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var command = new UpdateMealCommand(updateMealDto, anotherUserId);

            var dish = new FoodService.Domain.Entities.Dish
            {
                Id = updateMealDto.Dishes[0].FoodId,
                Name = "Old Dish",
                ImageUrl = "old_image_url"
            };

            var day = new FoodService.Domain.Entities.DayResult
            {
                Id = updateMealDto.DayId,
                ProfileId = ownerId,
                Meals = new List<FoodService.Domain.Entities.Meal>
                {
                    new FoodService.Domain.Entities.Meal { Id = updateMealDto.Id }
                }
            };

            _unitOfWorkMock
                .Setup(u => u.DayResultRepository.GetByIdAsync(updateMealDto.DayId))
                .ReturnsAsync(day);

            _userServiceMock
                .Setup(u => u.CheckProfileBelonging(anotherUserId, day.ProfileId))
                .ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Forbidden>()
                .WithMessage("You dont have access to this meal");

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
    }
}
