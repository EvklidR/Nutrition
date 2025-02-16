using AutoMapper;
using Bogus;
using FluentAssertions;
using FoodService.Application.DTOs.DayResult;
using FoodService.Application.Exceptions;
using FoodService.Application.Interfaces;
using FoodService.Application.UseCases.CommandHandlers.DayResult;
using FoodService.Application.UseCases.Commands.DayResult;
using FoodService.Domain.Interfaces;
using Moq;

namespace FoodServiceTests.DayResult
{
    public class CreateDayResultTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUserService> _userServiceMock;

        private readonly CreateDayResultHandler _handler;

        private readonly Faker _faker;

        public CreateDayResultTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _userServiceMock = new Mock<IUserService>();

            _faker = new Faker();

            _handler = new CreateDayResultHandler(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _userServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateDayResult_WhenUserHasAccess()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var profileId = _faker.Random.Guid();
            var dayResultId = _faker.Random.Guid();

            var createDayResultDTO = new CreateDayResultDTO()
            {
                ProfileId = profileId,
            };

            var dayResultEntity = new FoodService.Domain.Entities.DayResult()
            {
                Id = dayResultId,
            };

            var expectedDto = new DayResultDTO()
            {
                Id = dayResultId,
            };

            var command = new CreateDayResultCommand(createDayResultDTO, userId);

            _userServiceMock
                .Setup(s => s.CheckProfileBelonging(userId, createDayResultDTO.ProfileId))
                .ReturnsAsync(true);

            _mapperMock
                .Setup(m => m.Map<FoodService.Domain.Entities.DayResult>(command.CreateDayResultDTO))
                .Returns(dayResultEntity);

            _mapperMock
                .Setup(m => m.Map<DayResultDTO>(dayResultEntity))
                .Returns(expectedDto);

            _unitOfWorkMock
                .Setup(u => u.DayResultRepository.Add(dayResultEntity));

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedDto);

            _unitOfWorkMock.Verify(u => u.DayResultRepository.Add(It.IsAny<FoodService.Domain.Entities.DayResult>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowForbiddenException_WhenUserHasNoAccess()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var profileId = _faker.Random.Guid();

            var createDayResultDTO = new CreateDayResultDTO()
            {
                ProfileId = profileId,
            };

            var command = new CreateDayResultCommand(createDayResultDTO, userId);

            _userServiceMock
                .Setup(s => s.CheckProfileBelonging(userId, profileId))
                .ReturnsAsync(false);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Forbidden>()
                .WithMessage("You dont have access to this meal");

            _unitOfWorkMock.Verify(u => u.DayResultRepository.Add(It.IsAny<FoodService.Domain.Entities.DayResult>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
    }
}
