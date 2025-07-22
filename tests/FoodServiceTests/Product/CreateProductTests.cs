using AutoMapper;
using Bogus;
using FluentAssertions;
using FoodService.Application.UseCases.Commands.Product;
using FoodService.Application.UseCases.CommandHandlers.Product;
using FoodService.Application.Interfaces;
using FoodService.Domain.Interfaces;
using Moq;
using FoodService.Application.Exceptions;
using FoodService.Application.DTOs.Product.Requests;

namespace FoodServiceTests.Product
{
    public class CreateProductTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICheckUserService> _userServiceMock;
        private readonly IMapper _mapper;

        private readonly CreateProductHandler _handler;

        private readonly Faker<CreateProductDTO> _createProductDtoFaker;

        public CreateProductTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userServiceMock = new Mock<ICheckUserService>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FoodService.Application.Mappers.ProductMappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _createProductDtoFaker = new Faker<CreateProductDTO>()
                .RuleFor(p => p.UserId, f => f.Random.Guid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Proteins, f => f.Random.Int(1, 100))
                .RuleFor(p => p.Fats, f => f.Random.Int(1, 100))
                .RuleFor(p => p.Carbohydrates, f => f.Random.Int(1, 100));

            _handler = new CreateProductHandler(
                _unitOfWorkMock.Object,
                _mapper,
                _userServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateProductSuccessfully()
        {
            // Arrange
            var createProductDto = _createProductDtoFaker.Generate();
            var command = new CreateProductCommand(createProductDto);

            var expectedCalories = 
                createProductDto.Proteins * 4
                + createProductDto.Fats * 9
                + createProductDto.Carbohydrates * 4;

            _userServiceMock
                .Setup(u => u.CheckUserByIdAsync((Guid)createProductDto.UserId))
                .ReturnsAsync(true);

            _unitOfWorkMock
                .Setup(u => u.ProductRepository.Add(It.IsAny<FoodService.Domain.Entities.Product>()));

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(createProductDto.Name);
            result.Calories.Should().Be(expectedCalories);
            result.Proteins.Should().Be(createProductDto.Proteins);
            result.Fats.Should().Be(createProductDto.Fats);
            result.Carbohydrates.Should().Be(createProductDto.Carbohydrates);

            _unitOfWorkMock.Verify(u => u.ProductRepository.Add(It.IsAny<FoodService.Domain.Entities.Product>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowForbidden_WhenUserDoesNotExist()
        {
            // Arrange
            var createProductDto = _createProductDtoFaker.Generate();

            var command = new CreateProductCommand(createProductDto);

            _userServiceMock
                .Setup(u => u.CheckUserByIdAsync((Guid)createProductDto.UserId))
                .ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Forbidden>()
                .WithMessage("This user doesn't exist");
        }
    }
}
