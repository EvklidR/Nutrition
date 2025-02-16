using AutoMapper;
using Bogus;
using FluentAssertions;
using FoodService.Application.UseCases.Queries.Dish;
using FoodService.Application.UseCases.QueryHandlers.Dish;
using FoodService.Domain.Interfaces;
using FoodService.Domain.Repositories.Models;
using Moq;

namespace FoodServiceTests.Dish
{
    public class GetDishesTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly GetDishesHandler _handler;

        private readonly Faker<FoodService.Domain.Entities.Dish> _dishFaker;

        public GetDishesTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FoodService.Application.Mappers.DishMappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _dishFaker = new Faker<FoodService.Domain.Entities.Dish>()
                .RuleFor(d => d.Id, f => f.Random.Guid())
                .RuleFor(d => d.UserId, f => f.Random.Guid());

            _handler = new GetDishesHandler(_unitOfWorkMock.Object, _mapper);
        }

        [Fact]
        public async Task Handle_ShouldReturnDishes_WhenDishesExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var requestParams = new GetFoodRequestParameters(null, Page: 1, PageSize: 10, null);

            var command = new GetDishesQuery(userId, requestParams);

            var dishes = _dishFaker.Generate(3);
            var repositoryResponse = (dishes, totalCount: dishes.Count);

            _unitOfWorkMock
                .Setup(u => u.DishRepository.GetAllAsync(userId, requestParams))
                .ReturnsAsync(repositoryResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Dishes.Should().HaveCount(3);
            result.TotalCount.Should().Be(3);

            _unitOfWorkMock.Verify(u => u.DishRepository.GetAllAsync(userId, requestParams), Times.Once);
        }
    }
}
