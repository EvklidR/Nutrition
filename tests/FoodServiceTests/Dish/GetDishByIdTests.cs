using AutoMapper;
using Bogus;
using FluentAssertions;
using FoodService.Application.UseCases.Queries.Dish;
using FoodService.Application.UseCases.QueryHandlers.Dish;
using FoodService.Application.Exceptions;
using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces;
using Moq;

namespace FoodServiceTests.Dish
{
    public class GetDishByIdTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly GetDishByIdHandler _handler;

        private readonly Faker<FoodService.Domain.Entities.Dish> _dishFaker;

        public GetDishByIdTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FoodService.Application.Mappers.DishMappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _dishFaker = new Faker<FoodService.Domain.Entities.Dish>()
                .RuleFor(d => d.Id, f => f.Random.Guid())
                .RuleFor(d => d.UserId, f => f.Random.Guid())
                .RuleFor(d => d.Name, f => f.Lorem.Word())
                .RuleFor(d => d.Description, f => f.Lorem.Sentence())
                .RuleFor(d => d.ImageUrl, f => f.Internet.Url())
                .RuleFor(d => d.Ingredients, f => new List<ProductOfRecipe>());

            _handler = new GetDishByIdHandler(_unitOfWorkMock.Object, _mapper);
        }

        [Fact]
        public async Task Handle_ShouldReturnDish_WhenDishExistsAndUserIsOwner()
        {
            // Arrange
            var dish = _dishFaker.Generate();
            var command = new GetDishByIdQuery(dish.Id, dish.UserId);

            _unitOfWorkMock
                .Setup(u => u.DishRepository.GetByIdAsync(dish.Id))
                .ReturnsAsync(dish);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(dish.Id);
            result.Name.Should().Be(dish.Name);
            result.Description.Should().Be(dish.Description);
            result.ImageUrl.Should().Be(dish.ImageUrl);

            _unitOfWorkMock.Verify(u => u.DishRepository.GetByIdAsync(dish.Id), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenDishDoesNotExist()
        {
            // Arrange
            var dishId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new GetDishByIdQuery(dishId, userId);

            _unitOfWorkMock
                .Setup(u => u.DishRepository.GetByIdAsync(dishId))
                .ReturnsAsync((FoodService.Domain.Entities.Dish)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFound>()
                .WithMessage("Dish not found");

            _unitOfWorkMock.Verify(u => u.DishRepository.GetByIdAsync(dishId), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowForbidden_WhenUserIsNotOwner()
        {
            // Arrange
            var dish = _dishFaker.Generate();
            var anotherUserId = Guid.NewGuid();
            var command = new GetDishByIdQuery(dish.Id, anotherUserId);

            _unitOfWorkMock
                .Setup(u => u.DishRepository.GetByIdAsync(dish.Id))
                .ReturnsAsync(dish);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Forbidden>()
                .WithMessage("You dont have access to this dish");

            _unitOfWorkMock.Verify(u => u.DishRepository.GetByIdAsync(dish.Id), Times.Once);
        }
    }
}
