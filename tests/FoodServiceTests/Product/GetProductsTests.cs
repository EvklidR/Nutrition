using AutoMapper;
using FluentAssertions;
using Moq;
using FoodService.Application.UseCases.Queries.Product;
using FoodService.Application.UseCases.QueryHandlers.Product;
using FoodService.Domain.Interfaces;
using FoodService.Domain.Repositories.Models;
using Bogus;
using FoodService.Application.DTOs.Product.Responses;

namespace FoodServiceTests.Product
{
    public class GetProductsTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMapper _mapper;

        private readonly GetProductsHandler _handler;

        private readonly Faker _faker;

        public GetProductsTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _faker = new Faker();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FoodService.Domain.Entities.Product, ProductResponse>();
            });
            _mapper = mapperConfig.CreateMapper();

            _handler = new GetProductsHandler(_unitOfWorkMock.Object, _mapper);
        }

        [Fact]
        public async Task Handle_ShouldReturnProducts_WhenProductsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetProductsQuery(userId, new GetFoodRequestParameters(null, null, null, null));

            var products = new List<FoodService.Domain.Entities.Product>
            {
                new FoodService.Domain.Entities.Product { Id = Guid.NewGuid(), Name = _faker.Random.Word() },
                new FoodService.Domain.Entities.Product { Id = Guid.NewGuid(), Name = _faker.Random.Word() }
            };

            var response = (products, products.Count);

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetAllAsync(userId, query.Parameters))
                .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Products.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoProductsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetProductsQuery(userId, new GetFoodRequestParameters(null, null, null, null));

            var response = (new List<FoodService.Domain.Entities.Product>(), 0);

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetAllAsync(userId, query.Parameters))
                .ReturnsAsync(response);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Products.Should().BeEmpty();
            result.TotalCount.Should().Be(0);
        }
    }
}
