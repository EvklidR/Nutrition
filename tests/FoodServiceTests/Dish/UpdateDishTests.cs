using AutoMapper;
using Bogus;
using FluentAssertions;
using FoodService.Application.UseCases.Commands.Dish;
using FoodService.Application.UseCases.CommandHandlers.Dish;
using FoodService.Application.Interfaces;
using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces;
using Moq;
using FoodService.Application.Exceptions;
using Microsoft.AspNetCore.Http.Internal;
using FoodService.Application.DTOs.Recipe.Requests;

namespace FoodServiceTests.Dish
{
    public class UpdateDishTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IImageService> _imageServiceMock;
        private readonly IMapper _mapper;

        private readonly UpdateDishHandler _handler;

        private readonly Faker<UpdateRecipeDTO> _updateDishDtoFaker;

        public UpdateDishTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _imageServiceMock = new Mock<IImageService>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FoodService.Application.Mappers.DishMappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            _updateDishDtoFaker = new Faker<UpdateRecipeDTO>()
                .RuleFor(d => d.Id, f => f.Random.Guid())
                .RuleFor(d => d.Name, f => f.Lorem.Word())
                .RuleFor(d => d.Description, f => f.Lorem.Sentence())
                .RuleFor(d => d.AmountOfPortions, f => f.Random.Int(1, 5))
                .RuleFor(d => d.Image, f => new FormFile(new MemoryStream(), 0, 0, "image", "test.jpg"))
                .RuleFor(d => d.Ingredients, f => new List<CreateOrUpdateProductOfRecipeDTO>
                {
                    new CreateOrUpdateProductOfRecipeDTO { ProductId = Guid.NewGuid(), Weight = 300 },
                    new CreateOrUpdateProductOfRecipeDTO { ProductId = Guid.NewGuid(), Weight = 150 }
                });

            _handler = new UpdateDishHandler(_unitOfWorkMock.Object, _mapper, _imageServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateDish_WhenDishExistsAndUserIsOwner()
        {
            // Arrange
            var updateDishDto = _updateDishDtoFaker.Generate();
            var userId = Guid.NewGuid();
            var command = new UpdateDishCommand(updateDishDto, userId);

            int amountOfPortions = updateDishDto.AmountOfPortions;

            var product1 = new FoodService.Domain.Entities.Product { Id = updateDishDto.Ingredients[0].ProductId, Calories = 120, Proteins = 10, Fats = 5, Carbohydrates = 20 };
            var product2 = new FoodService.Domain.Entities.Product { Id = updateDishDto.Ingredients[1].ProductId, Calories = 200, Proteins = 15, Fats = 10, Carbohydrates = 30 };

            var dish = new FoodService.Domain.Entities.Dish
            {
                Id = updateDishDto.Id,
                UserId = userId,
                ImageUrl = "old_image_url",
            };

            _unitOfWorkMock
                .Setup(u => u.DishRepository.GetByIdAsync(updateDishDto.Id))
                .ReturnsAsync(dish);

            _imageServiceMock
                .Setup(i => i.UploadImageAsync(updateDishDto.Image))
                .ReturnsAsync("new_image_url");

            _unitOfWorkMock
                .Setup(u => u.ProductRepository.GetByIdAsync(updateDishDto.Ingredients[0].ProductId))
                .ReturnsAsync(product1);

            _unitOfWorkMock
                .Setup(u => u.ProductRepository.GetByIdAsync(updateDishDto.Ingredients[1].ProductId))
                .ReturnsAsync(product2);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            _imageServiceMock.Verify(i => i.UploadImageAsync(updateDishDto.Image), Times.Once);
            _imageServiceMock.Verify(i => i.DeleteImageAsync("old_image_url"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenDishDoesNotExist()
        {
            // Arrange
            var updateDishDto = _updateDishDtoFaker.Generate();
            var userId = Guid.NewGuid();
            var command = new UpdateDishCommand(updateDishDto, userId);

            _unitOfWorkMock
                .Setup(u => u.DishRepository.GetByIdAsync(updateDishDto.Id))
                .ReturnsAsync((FoodService.Domain.Entities.Dish)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFound>()
                .WithMessage("Dish not found");

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowForbidden_WhenUserIsNotOwner()
        {
            // Arrange
            var updateDishDto = _updateDishDtoFaker.Generate();
            var ownerId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var command = new UpdateDishCommand(updateDishDto, anotherUserId);

            var dish = new FoodService.Domain.Entities.Dish
            {
                Id = updateDishDto.Id,
                UserId = ownerId,
                ImageUrl = "old_image_url",
                Ingredients = new List<ProductOfRecipe>()
            };

            _unitOfWorkMock
                .Setup(u => u.DishRepository.GetByIdAsync(updateDishDto.Id))
                .ReturnsAsync(dish);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Forbidden>()
                .WithMessage("You dont have access to this entity");

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
    }
}
