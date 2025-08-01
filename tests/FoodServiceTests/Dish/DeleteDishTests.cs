using FluentAssertions;
using FoodService.Application.Exceptions;
using FoodService.Application.Interfaces;
using FoodService.Application.UseCases.Commands.Dish;
using FoodService.Application.UseCases.CommandHandlers.Dish;
using FoodService.Domain.Interfaces;
using Moq;

namespace FoodServiceTests.Dish
{
    public class DeleteDishTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IImageService> _imageServiceMock;

        private readonly DeleteRecipeHandler _handler;

        public DeleteDishTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _imageServiceMock = new Mock<IImageService>();

            _handler = new DeleteRecipeHandler(_unitOfWorkMock.Object, _imageServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteDish_WhenDishExistsAndUserIsOwner()
        {
            // Arrange
            var dishId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var dish = new FoodService.Domain.Entities.Dish
            {
                Id = dishId,
                UserId = userId,
                ImageUrl = "fake_image_url"
            };

            _unitOfWorkMock
                .Setup(u => u.DishRepository.GetByIdAsync(dishId))
                .ReturnsAsync(dish);

            _imageServiceMock
                .Setup(i => i.DeleteImageAsync(dish.ImageUrl))
                .ReturnsAsync(true);

            _unitOfWorkMock
                .Setup(u => u.DishRepository.Delete(dish));

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var command = new DeleteRecipeCommand(dishId, userId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _unitOfWorkMock.Verify(u => u.DishRepository.Delete(dish), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            _imageServiceMock.Verify(i => i.DeleteImageAsync(dish.ImageUrl), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenDishDoesNotExist()
        {
            // Arrange
            var dishId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _unitOfWorkMock
                .Setup(u => u.DishRepository.GetByIdAsync(dishId))
                .ReturnsAsync((FoodService.Domain.Entities.Dish)null);

            var command = new DeleteRecipeCommand(dishId, userId);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFound>()
                .WithMessage("Dish not found");

            _unitOfWorkMock.Verify(u => u.DishRepository.Delete(It.IsAny<FoodService.Domain.Entities.Dish>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
            _imageServiceMock.Verify(i => i.DeleteImageAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowForbidden_WhenUserIsNotOwner()
        {
            // Arrange
            var dishId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var dish = new FoodService.Domain.Entities.Dish
            {
                Id = dishId,
                UserId = ownerId,
                ImageUrl = "fake_image_url"
            };

            _unitOfWorkMock
                .Setup(u => u.DishRepository.GetByIdAsync(dishId))
                .ReturnsAsync(dish);

            var command = new DeleteRecipeCommand(dishId, anotherUserId);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Forbidden>()
                .WithMessage("You dont have access to this dish");

            _unitOfWorkMock.Verify(u => u.DishRepository.Delete(It.IsAny<FoodService.Domain.Entities.Dish>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
            _imageServiceMock.Verify(i => i.DeleteImageAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
