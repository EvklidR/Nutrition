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
    public class GetOrCreateDayResultTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly GetOrCreateDayResultHandler _handler;

        private readonly Faker _faker;
        private readonly Faker<FoodService.Domain.Entities.DayResult> _dayResultFaker;

        public GetOrCreateDayResultTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userServiceMock = new Mock<IUserService>();
            _mapperMock = new Mock<IMapper>();
            _faker = new Faker();

            _dayResultFaker = new Faker<FoodService.Domain.Entities.DayResult>()
                .RuleFor(d => d.Id, f => f.Random.Guid())
                .RuleFor(d => d.ProfileId, f => f.Random.Guid())
                .RuleFor(d => d.Date, f => DateOnly.FromDateTime(DateTime.Now))
                .RuleFor(d => d.GlassesOfWater, 0);

            _handler = new GetOrCreateDayResultHandler(
                _unitOfWorkMock.Object,
                _userServiceMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnExistingDayResult_WhenFound()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var profileId = _faker.Random.Guid();
            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            var existingDayResult = _dayResultFaker.Generate();
            existingDayResult.ProfileId = profileId;

            var expectedDto = new DayResultDTO { Id = existingDayResult.Id };

            var command = new GetOrCreateDayResultCommand(profileId, userId);

            _userServiceMock
                .Setup(s => s.CheckProfileBelonging(userId, profileId))
                .ReturnsAsync(true);

            _unitOfWorkMock
                .Setup(u => u.DayResultRepository.GetByDateAsync(profileId, currentDate))
                .ReturnsAsync(existingDayResult);

            _mapperMock
                .Setup(m => m.Map<DayResultDTO>(existingDayResult))
                .Returns(expectedDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedDto);

            _unitOfWorkMock.Verify(u => u.DayResultRepository.Add(It.IsAny<FoodService.Domain.Entities.DayResult>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCreateNewDayResult_WhenNotFound()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var profileId = _faker.Random.Guid();
            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            var newDayResult = _dayResultFaker.Generate();
            newDayResult.ProfileId = profileId;

            var expectedDto = new DayResultDTO { Id = newDayResult.Id };

            var command = new GetOrCreateDayResultCommand(profileId, userId);

            _userServiceMock
                .Setup(s => s.CheckProfileBelonging(userId, profileId))
                .ReturnsAsync(true);

            _unitOfWorkMock
                .Setup(u => u.DayResultRepository.GetByDateAsync(profileId, currentDate))
                .ReturnsAsync((FoodService.Domain.Entities.DayResult)null);

            _mapperMock
                .Setup(m => m.Map<DayResultDTO>(It.IsAny<FoodService.Domain.Entities.DayResult>()))
                .Returns(expectedDto);

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

            var command = new GetOrCreateDayResultCommand(profileId, userId);

            _userServiceMock
                .Setup(s => s.CheckProfileBelonging(userId, profileId))
                .ReturnsAsync(false);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Forbidden>()
                .WithMessage("You dont have access to this meal");

            _unitOfWorkMock.Verify(u => u.DayResultRepository.GetByDateAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.DayResultRepository.Add(It.IsAny<FoodService.Domain.Entities.DayResult>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
    }
}
