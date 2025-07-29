using Bogus;
using FluentAssertions;
using FoodService.Application.Exceptions;
using FoodService.Application.Interfaces;
using FoodService.Application.UseCases.CommandHandlers.DayResult;
using FoodService.Application.UseCases.Commands.DayResult;
using FoodService.Domain.Interfaces;
using Moq;

namespace FoodServiceTests.DayResult
{
    public class DeleteDayResultTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUserService> _userServiceMock;

        private readonly DeleteDayResultHandler _handler;

        private readonly Faker _faker;

        public DeleteDayResultTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userServiceMock = new Mock<IUserService>();

            _faker = new Faker();

            _handler = new DeleteDayResultHandler(
                _unitOfWorkMock.Object,
                _userServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteDayResult_WhenUserHasAccess()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var profileId = _faker.Random.Guid();
            var dayResultId = _faker.Random.Guid();

            var dayResult = new Faker<FoodService.Domain.Entities.DayResult>()
                .RuleFor(d => d.Id, dayResultId)
                .RuleFor(d => d.ProfileId, profileId)
                .Generate();

            var command = new DeleteDayResultCommand(dayResultId, userId);

            _unitOfWorkMock
                .Setup(u => u.DayResultRepository.GetByIdAsync(dayResultId))
                .ReturnsAsync(dayResult);

            _userServiceMock
                .Setup(s => s.CheckProfileBelonging(userId, profileId))
                .ReturnsAsync(true);

            _unitOfWorkMock
                .Setup(u => u.DayResultRepository.Delete(dayResult));

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _unitOfWorkMock.Verify(u => u.DayResultRepository.Delete(It.IsAny<FoodService.Domain.Entities.DayResult>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenDayResultDoesNotExist()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var dayResultId = _faker.Random.Guid();

            var command = new DeleteDayResultCommand(dayResultId, userId);

            _unitOfWorkMock
                .Setup(u => u.DayResultRepository.GetByIdAsync(dayResultId))
                .ReturnsAsync((FoodService.Domain.Entities.DayResult)null);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFound>()
                .WithMessage("DayResult not found");

            _unitOfWorkMock.Verify(u => u.DayResultRepository.Delete(It.IsAny<FoodService.Domain.Entities.DayResult>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowForbiddenException_WhenUserHasNoAccess()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var profileId = _faker.Random.Guid();
            var dayResultId = _faker.Random.Guid();

            var dayResult = new Faker<FoodService.Domain.Entities.DayResult>()
                .RuleFor(d => d.Id, dayResultId)
                .RuleFor(d => d.ProfileId, profileId)
                .Generate();

            var command = new DeleteDayResultCommand(dayResultId, userId);

            _unitOfWorkMock
                .Setup(u => u.DayResultRepository.GetByIdAsync(dayResultId))
                .ReturnsAsync(dayResult);

            _userServiceMock
                .Setup(s => s.CheckProfileBelonging(userId, profileId))
                .ReturnsAsync(false);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Forbidden>()
                .WithMessage("You dont have access to this meal");

            _unitOfWorkMock.Verify(u => u.DayResultRepository.Delete(It.IsAny<FoodService.Domain.Entities.DayResult>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
    }
}
