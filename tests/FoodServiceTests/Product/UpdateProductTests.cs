using FluentAssertions;
using Moq;
using AutoMapper;
using FoodService.Application.DTOs.Product;
using FoodService.Application.Exceptions;
using FoodService.Application.UseCases.Commands.Product;
using FoodService.Application.UseCases.CommandHandlers.Product;
using FoodService.Domain.Interfaces;
using Bogus;

namespace FoodServiceTests.Product
{
    public class UpdateProductTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMapper _mapper;

        private readonly UpdateProductHandler _handler;

        private readonly Faker _faker;

        public UpdateProductTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FoodService.Application.Mappers.ProductMappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _handler = new UpdateProductHandler(_unitOfWorkMock.Object, _mapper);

            _faker = new Faker();
        }

        [Fact]
        public async Task Handle_ShouldUpdateProductSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = new FoodService.Domain.Entities.Product
            {
                Id = productId,
                UserId = userId,
                Name = _faker.Random.Word()
            };

            var updateDto = new UpdateProductDTO
            {
                Id = productId,
                Name = _faker.Random.Word()
            };

            var command = new UpdateProductCommand(updateDto, userId);

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetByIdAsync(productId))
                .ReturnsAsync(product);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var command = new UpdateProductCommand(new UpdateProductDTO { Id = Guid.NewGuid() }, Guid.NewGuid());

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetByIdAsync(command.UpdateProductDTO.Id))
                .ReturnsAsync((FoodService.Domain.Entities.Product)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFound>()
                .WithMessage("Ingredient not found");
        }

        [Fact]
        public async Task Handle_ShouldThrowForbidden_WhenUserHasNoAccess()
        {
            // Arrange
            var product = new FoodService.Domain.Entities.Product
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            var command = new UpdateProductCommand(new UpdateProductDTO { Id = product.Id }, Guid.NewGuid());

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetByIdAsync(product.Id))
                .ReturnsAsync(product);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Forbidden>()
                .WithMessage("You dont have access to this product");
        }
    }
}
