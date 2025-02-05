using AutoMapper;
using Bogus;
using FluentAssertions;
using FoodService.Application.DTOs.DayResult;
using FoodService.Application.Exceptions;
using FoodService.Application.Interfaces;
using FoodService.Application.UseCases.Queries.DayResult;
using FoodService.Application.UseCases.QueryHandlers.DayResult;
using FoodService.Domain.Interfaces;
using Moq;

namespace FoodServiceTests.DayResult
{
    public class GetDayResultsByPeriodTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly GetDayResultsByPeriodQueryHandler _handler;

        private readonly Faker _faker;
        private readonly Faker<FoodService.Domain.Entities.DayResult> _dayResultFaker;

        public GetDayResultsByPeriodTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userServiceMock = new Mock<IUserService>();
            _mapperMock = new Mock<IMapper>();

            _faker = new Faker();

            _dayResultFaker = new Faker<FoodService.Domain.Entities.DayResult>()
                .RuleFor(d => d.Id, f => f.Random.Guid())
                .RuleFor(d => d.ProfileId, f => f.Random.Guid())
                .RuleFor(d => d.Date, f => DateOnly.FromDateTime(f.Date.Between(DateTime.Now.AddMonths(-1), DateTime.Now)))
                .RuleFor(d => d.GlassesOfWater, f => f.Random.Int(0, 10));

            _handler = new GetDayResultsByPeriodQueryHandler(
                _unitOfWorkMock.Object,
                _userServiceMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnDayResults_WhenUserHasAccess()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var profileId = _faker.Random.Guid();
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-7));
            var endDate = DateOnly.FromDateTime(DateTime.Now);

            var query = new GetDayResultsByPeriodQuery(profileId, startDate, endDate, userId);

            var dayResults = _dayResultFaker.Generate(5);
            var expectedDtos = new List<DayResultDTO>();

            _userServiceMock
                .Setup(s => s.CheckProfileBelonging(userId, profileId))
                .ReturnsAsync(true);

            _unitOfWorkMock
                .Setup(u => u.DayResultRepository.GetAllByPeriodAsync(profileId, startDate, endDate))
                .ReturnsAsync(dayResults);

            _mapperMock
                .Setup(m => m.Map<List<DayResultDTO>>(dayResults))
                .Returns(expectedDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedDtos);

            _userServiceMock.Verify(s => s.CheckProfileBelonging(userId, profileId), Times.Once);
            _unitOfWorkMock.Verify(u => u.DayResultRepository.GetAllByPeriodAsync(profileId, startDate, endDate), Times.Once);
            _mapperMock.Verify(m => m.Map<List<DayResultDTO>>(dayResults), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowForbiddenException_WhenUserHasNoAccess()
        {
            // Arrange
            var userId = _faker.Random.Guid();
            var profileId = _faker.Random.Guid();
            var startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-7));
            var endDate = DateOnly.FromDateTime(DateTime.Now);

            var query = new GetDayResultsByPeriodQuery(profileId, startDate, endDate, userId);

            _userServiceMock
                .Setup(s => s.CheckProfileBelonging(userId, profileId))
                .ReturnsAsync(false);

            // Act
            var act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Forbidden>()
                .WithMessage("You dont have access to this day result");

            _unitOfWorkMock.Verify(u => u.DayResultRepository.GetAllByPeriodAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>()), Times.Never);
            _mapperMock.Verify(m => m.Map<List<DayResultDTO>>(It.IsAny<List<FoodService.Domain.Entities.DayResult>>()), Times.Never);
        }
    }
}
