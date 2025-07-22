using Moq;
using FoodService.Application.Exceptions;
using FoodService.Domain.Interfaces;
using FoodService.Application.UseCases.Commands.Meal;
using FoodService.Application.UseCases.CommandHandlers.Meal;
using FoodService.Application.Interfaces;
using FluentAssertions;

namespace FoodServiceTests.Meal
{
    public class DeleteMealHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICheckUserService> _userServiceMock;
        private readonly DeleteMealHandler _handler;

        public DeleteMealHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userServiceMock = new Mock<ICheckUserService>();
            _handler = new DeleteMealHandler(_unitOfWorkMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteMealSuccessfully()
        {
            // Arrange
            var mealId = Guid.NewGuid();
            var dayId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var day = new FoodService.Domain.Entities.DayResult
            {
                Id = dayId,
                ProfileId = Guid.NewGuid(),
                Meals = new List<FoodService.Domain.Entities.Meal>
                {
                    new FoodService.Domain.Entities.Meal { Id = mealId }
                }
            };

            _unitOfWorkMock.Setup(u => u.DayResultRepository.GetByIdAsync(dayId))
                .ReturnsAsync(day);

            _userServiceMock.Setup(u => u.CheckProfileBelonging(userId, day.ProfileId))
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            var command = new DeleteMealCommand(mealId, dayId, userId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            day.Meals.Should().BeEmpty();
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenDayDoesNotExist()
        {
            // Arrange
            var mealId = Guid.NewGuid();
            var dayId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _unitOfWorkMock.Setup(u => u.DayResultRepository.GetByIdAsync(dayId))
                .ReturnsAsync((FoodService.Domain.Entities.DayResult)null);

            var command = new DeleteMealCommand(mealId, dayId, userId);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFound>().WithMessage("Day not found");
        }

        [Fact]
        public async Task Handle_ShouldThrowForbidden_WhenUserHasNoAccess()
        {
            // Arrange
            var mealId = Guid.NewGuid();
            var dayId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var profileId = Guid.NewGuid();

            var day = new FoodService.Domain.Entities.DayResult
            {
                Id = dayId,
                ProfileId = profileId,
                Meals = new List<FoodService.Domain.Entities.Meal>
                {
                    new FoodService.Domain.Entities.Meal { Id = mealId }
                }
            };

            _unitOfWorkMock.Setup(u => u.DayResultRepository.GetByIdAsync(dayId))
                .ReturnsAsync(day);

            _userServiceMock.Setup(u => u.CheckProfileBelonging(userId, profileId))
                .ReturnsAsync(false);

            var command = new DeleteMealCommand(mealId, dayId, userId);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Forbidden>().WithMessage("You dont have access to this meal");
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenMealDoesNotExist()
        {
            // Arrange
            var mealId = Guid.NewGuid();
            var dayId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var day = new FoodService.Domain.Entities.DayResult
            {
                Id = dayId,
                ProfileId = Guid.NewGuid(),
                Meals = new List<FoodService.Domain.Entities.Meal>()
            };

            _unitOfWorkMock.Setup(u => u.DayResultRepository.GetByIdAsync(dayId))
                .ReturnsAsync(day);

            _userServiceMock.Setup(u => u.CheckProfileBelonging(userId, day.ProfileId))
                .ReturnsAsync(true);

            var command = new DeleteMealCommand(mealId, dayId, userId);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFound>().WithMessage("Meal not found");
        }
    }
}
