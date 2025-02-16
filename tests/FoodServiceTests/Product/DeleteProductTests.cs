using FluentAssertions;
using FoodService.Application.Exceptions;
using FoodService.Application.UseCases.Commands.Product;
using FoodService.Application.UseCases.CommandHandlers.Product;
using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces;
using Moq;

namespace FoodServiceTests.Product
{
    public class DeleteProductTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        private readonly DeleteProductHandler _handler;

        public DeleteProductTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new DeleteProductHandler(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteProductSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = new FoodService.Domain.Entities.Product
            {
                Id = productId,
                UserId = userId
            };

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetByIdAsync(productId))
                .ReturnsAsync(product);

            _unitOfWorkMock.Setup(u => u.DishRepository.GetAllAsync(userId))
                .ReturnsAsync(new List<FoodService.Domain.Entities.Dish>());

            _unitOfWorkMock.Setup(u => u.DayResultRepository.GetAllAsync(userId))
                .ReturnsAsync(new List<FoodService.Domain.Entities.DayResult>());

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(new DeleteProductCommand(productId, userId), CancellationToken.None);

            // Assert
            _unitOfWorkMock.Verify(u => u.ProductRepository.Delete(product), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var command = new DeleteProductCommand(Guid.NewGuid(), Guid.NewGuid());

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetByIdAsync(command.ProductId))
                .ReturnsAsync((FoodService.Domain.Entities.Product)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFound>()
                .WithMessage("Product not found");
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

            var command = new DeleteProductCommand(product.Id, Guid.NewGuid());

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetByIdAsync(product.Id))
                .ReturnsAsync(product);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Forbidden>()
                .WithMessage("You dont have access to this product");
        }

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenProductIsUsedInDish()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = new FoodService.Domain.Entities.Product
            {
                Id = productId,
                UserId = userId
            };

            var dishes = new List<FoodService.Domain.Entities.Dish>
            {
                new FoodService.Domain.Entities.Dish
                {
                    Ingredients = new List<ProductOfDish>
                    {
                        new ProductOfDish { ProductId = productId }
                    }
                }
            };

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetByIdAsync(productId))
                .ReturnsAsync(product);

            _unitOfWorkMock.Setup(u => u.DishRepository.GetAllAsync(userId))
                .ReturnsAsync(dishes);

            // Act
            Func<Task> act = async () => await _handler.Handle(new DeleteProductCommand(productId, userId), CancellationToken.None);

            // Assert
            var exception =  await act.Should().ThrowAsync<BadRequest>();

            exception.Which.Errors.Contains("This product there is in some dish");
        }

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenProductIsUsedInMeal()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = new FoodService.Domain.Entities.Product
            {
                Id = productId,
                UserId = userId
            };

            var dayResults = new List<FoodService.Domain.Entities.DayResult>
            {
                new FoodService.Domain.Entities.DayResult
                {
                    Meals = new List<FoodService.Domain.Entities.Meal>
                    {
                        new FoodService.Domain.Entities.Meal
                        {
                            Products = new List<EatenProduct>
                            {
                                new EatenProduct { FoodId = productId }
                            }
                        }
                    }
                }
            };

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetByIdAsync(productId))
                .ReturnsAsync(product);

            _unitOfWorkMock.Setup(u => u.DishRepository.GetAllAsync(userId))
                .ReturnsAsync(new List<FoodService.Domain.Entities.Dish>());

            _unitOfWorkMock.Setup(u => u.DayResultRepository.GetAllAsync(userId))
                .ReturnsAsync(dayResults);

            // Act
            Func<Task> act = async () => await _handler.Handle(new DeleteProductCommand(productId, userId), CancellationToken.None);

            // Assert
            var exception = await act.Should().ThrowAsync<BadRequest>();

            exception.Which.Errors.Contains("This product there is in some meals");
        }
    }
}
