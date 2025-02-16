using AutoMapper;
using Bogus;
using FluentAssertions;
using FoodService.Application.DTOs.Dish;
using FoodService.Application.UseCases.Commands.Dish;
using FoodService.Application.UseCases.CommandHandlers.Dish;
using FoodService.Application.Interfaces;
using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using FoodService.Application.Exceptions;

namespace FoodServiceTests.Dish
{
    public class CreateDishTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IImageService> _imageServiceMock;
        private readonly IMapper _mapper;

        private readonly CreateDishHandler _handler;

        private readonly Faker<CreateDishDTO> _createDishDtoFaker;

        public CreateDishTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userServiceMock = new Mock<IUserService>();
            _imageServiceMock = new Mock<IImageService>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FoodService.Application.Mappers.DishMappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _createDishDtoFaker = new Faker<CreateDishDTO>()
                .RuleFor(d => d.UserId, f => f.Random.Guid())
                .RuleFor(d => d.Name, f => f.Lorem.Word())
                .RuleFor(d => d.Description, f => f.Lorem.Sentence())
                .RuleFor(d => d.AmountOfPortions, f => f.Random.Int(1, 5))
                .RuleFor(d => d.Image, f => new FormFile(new MemoryStream(), 0, 0, "image", "test.jpg"))
                .RuleFor(d => d.Ingredients, f => new List<CreateOrUpdateProductOfDishDTO>
                {
                    new CreateOrUpdateProductOfDishDTO { ProductId = Guid.NewGuid(), Weight = 300 },
                    new CreateOrUpdateProductOfDishDTO { ProductId = Guid.NewGuid(), Weight = 150 }
                });

            _handler = new CreateDishHandler(
                _unitOfWorkMock.Object,
                _mapper,
                _userServiceMock.Object,
                _imageServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCalculateNutrientsCorrectly()
        {
            // Arrange
            var createDishDto = _createDishDtoFaker.Generate();
            var command = new CreateDishCommand(createDishDto);

            var product1 = new FoodService.Domain.Entities.Product { Id = createDishDto.Ingredients[0].ProductId, Calories = 120, Proteins = 10, Fats = 5, Carbohydrates = 20 };
            var product2 = new FoodService.Domain.Entities.Product { Id = createDishDto.Ingredients[1].ProductId, Calories = 200, Proteins = 15, Fats = 10, Carbohydrates = 30 };

            var expectedWeight1 = createDishDto.Ingredients[0].Weight / createDishDto.AmountOfPortions;
            var expectedWeight2 = createDishDto.Ingredients[1].Weight / createDishDto.AmountOfPortions;
            var totalWeight = expectedWeight1 + expectedWeight2;

            var expectedCalories = ((product1.Calories * expectedWeight1) + (product2.Calories * expectedWeight2)) / totalWeight;
            var expectedProteins = ((product1.Proteins * expectedWeight1) + (product2.Proteins * expectedWeight2)) / totalWeight;
            var expectedFats = ((product1.Fats * expectedWeight1) + (product2.Fats * expectedWeight2)) / totalWeight;
            var expectedCarbohydrates =
                ((product1.Carbohydrates * expectedWeight1)
                + (product2.Carbohydrates * expectedWeight2))
                / totalWeight;

            _userServiceMock
                .Setup(s => s.CheckUserByIdAsync(createDishDto.UserId))
                .ReturnsAsync(true);

            _imageServiceMock
                .Setup(i => i.UploadImageAsync(createDishDto.Image))
                .ReturnsAsync("fake_image_url");

            _unitOfWorkMock
                .Setup(u => u.ProductRepository.GetByIdAsync(createDishDto.Ingredients[0].ProductId))
                .ReturnsAsync(product1);

            _unitOfWorkMock
                .Setup(u => u.ProductRepository.GetByIdAsync(createDishDto.Ingredients[1].ProductId))
                .ReturnsAsync(product2);

            _unitOfWorkMock
                .Setup(u => u.DishRepository.Add(It.IsAny<FoodService.Domain.Entities.Dish>()));

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Calories.Should().BeApproximately(expectedCalories, 0.1);
            result.Proteins.Should().BeApproximately(expectedProteins, 0.1);
            result.Fats.Should().BeApproximately(expectedFats, 0.1);
            result.Carbohydrates.Should().BeApproximately(expectedCarbohydrates, 0.1);

            _userServiceMock.Verify(u => u.CheckUserByIdAsync(createDishDto.UserId), Times.Once);
            _unitOfWorkMock.Verify(u => u.DishRepository.Add(It.IsAny<FoodService.Domain.Entities.Dish>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            _imageServiceMock.Verify(i => i.UploadImageAsync(It.IsAny<IFormFile>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowForbidden_WhenUserDoesNotExist()
        {
            // Arrange
            var createDishDto = _createDishDtoFaker.Generate();
            var command = new CreateDishCommand(createDishDto);

            _userServiceMock
                .Setup(s => s.CheckUserByIdAsync(createDishDto.UserId))
                .ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Forbidden>()
                .WithMessage("This user doesn't exist");

            _userServiceMock.Verify(u => u.CheckUserByIdAsync(createDishDto.UserId), Times.Once);
            _unitOfWorkMock.Verify(u => u.DishRepository.Add(It.IsAny<FoodService.Domain.Entities.Dish>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
            _imageServiceMock.Verify(i => i.UploadImageAsync(It.IsAny<IFormFile>()), Times.Never);
        }

    }
}
