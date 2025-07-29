using AutoMapper;
using Bogus;
using FluentAssertions;
using FoodService.Application.DTOs.DayResult.Requests;
using FoodService.Application.Exceptions;
using FoodService.Application.Interfaces;
using FoodService.Application.UseCases.CommandHandlers.DayResult;
using FoodService.Application.UseCases.Commands.DayResult;
using FoodService.Domain.Interfaces;
using Moq;

namespace FoodServiceTests.DayResult
{
    public class UpdateDayResultTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly UpdateDayResultHandler _handler;

        private readonly Faker _faker;
        private readonly Faker<FoodService.Domain.Entities.DayResult> _dayResultFaker;
        private readonly Faker<UpdateDayResultDTO> _updateDayResultDtoFaker;

        public UpdateDayResultTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userServiceMock = new Mock<IUserService>();
            _mapperMock = new Mock<IMapper>();

            _faker = new Faker();

            _dayResultFaker = new Faker<FoodService.Domain.Entities.DayResult>()
                .RuleFor(d => d.Id, f => f.Random.Guid())
                .RuleFor(d => d.ProfileId, f => f.Random.Guid())
                .RuleFor(d => d.Date, f => DateOnly.FromDateTime(DateTime.Now))
                .RuleFor(d => d.GlassesOfWater, f => f.Random.Int(0, 10));

            _updateDayResultDtoFaker = new Faker<UpdateDayResultDTO>()
                .RuleFor(d => d.Id, f => f.Random.Guid())
                .RuleFor(d => d.GlassesOfWater, f => f.Random.Int(0, 10));

            _handler = new UpdateDayResultHandler(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _userServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateDayResult_WhenUserHasAccess()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var updateDto = _updateDayResultDtoFaker.Generate();
            var existingDayResult = _dayResultFaker.Generate();
            existingDayResult.Id = updateDto.Id;

            var command = new UpdateDayResultCommand(updateDto, userId);

            _unitOfWorkMock
                .Setup(u => u.DayResultRepository.GetByIdAsync(updateDto.Id))
                .ReturnsAsync(existingDayResult);

            _userServiceMock
                .Setup(s => s.CheckProfileBelonging(userId, existingDayResult.ProfileId))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mapperMock.Verify(m => m.Map(updateDto, existingDayResult), Times.Once);
            _unitOfWorkMock.Verify(u => u.DayResultRepository.Update(existingDayResult), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenDayResultDoesNotExist()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var updateDto = _updateDayResultDtoFaker.Generate();
            var command = new UpdateDayResultCommand(updateDto, userId);

            _unitOfWorkMock
                .Setup(u => u.DayResultRepository.GetByIdAsync(updateDto.Id))
                .ReturnsAsync((FoodService.Domain.Entities.DayResult)null);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFound>()
                .WithMessage("DayResult not found");

            _unitOfWorkMock.Verify(u => u.DayResultRepository.Update(It.IsAny<FoodService.Domain.Entities.DayResult>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowForbiddenException_WhenUserHasNoAccess()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var updateDto = _updateDayResultDtoFaker.Generate();
            var existingDayResult = _dayResultFaker.Generate();
            existingDayResult.Id = updateDto.Id;

            var command = new UpdateDayResultCommand(updateDto, userId);

            _unitOfWorkMock
                .Setup(u => u.DayResultRepository.GetByIdAsync(updateDto.Id))
                .ReturnsAsync(existingDayResult);

            _userServiceMock
                .Setup(s => s.CheckProfileBelonging(userId, existingDayResult.ProfileId))
                .ReturnsAsync(false);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Forbidden>()
                .WithMessage("You dont have access to this meal");

            _unitOfWorkMock.Verify(u => u.DayResultRepository.Update(It.IsAny<FoodService.Domain.Entities.DayResult>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
    }
}
