using AutoMapper;
using FoodService.Domain.Interfaces;
using FoodService.Application.Exceptions;
using FoodService.Application.UseCases.Commands.Dish;
using FoodService.Application.Interfaces;
using FoodService.Application.DTOs.Recipe.Requests;

namespace FoodService.Application.UseCases.CommandHandlers.Dish;

public class UpdateDishHandler : ICommandHandler<UpdateDishCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;

    public UpdateDishHandler(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageService = imageService;
    }

    public async Task Handle(UpdateDishCommand request, CancellationToken cancellationToken)
    {
        var dish = await _unitOfWork.DishRepository.GetByIdAsync(request.UpdateDishDTO.Id);

        if (dish == null)
        {
            throw new NotFound("Dish not found");
        }

        if (dish.UserId != request.UserId)
        {
            throw new Forbidden("You dont have access to this entity");
        }

        await UpdateImages(dish, request.UpdateDishDTO);

        _mapper.Map(request.UpdateDishDTO, dish);

        await CalculateNutrientsForDish(dish);

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task CalculateNutrientsForDish(Domain.Entities.Dish dish)
    {
        double weight = 0;

        dish.Calories = 0;
        dish.Proteins = 0;
        dish.Fats = 0;
        dish.Carbohydrates = 0;

        foreach (var ingredient in dish.Ingredients)
        {
            var ingredientBD = await _unitOfWork.ProductRepository.GetByIdAsync(ingredient.ProductId);

            if (ingredientBD == null)
            {
                throw new NotFound("Ingredient not found");
            }

            dish.Calories += ingredientBD.Calories * ingredient.WeightPerPortion;
            dish.Proteins += ingredientBD.Proteins * ingredient.WeightPerPortion;
            dish.Fats += ingredientBD.Fats * ingredient.WeightPerPortion;
            dish.Carbohydrates += ingredientBD.Carbohydrates * ingredient.WeightPerPortion;
            weight += ingredient.WeightPerPortion;
        }

        dish.Calories = dish.Calories / weight;
        dish.Fats = dish.Fats / weight;
        dish.Proteins = dish.Proteins / weight;
        dish.Carbohydrates = dish.Carbohydrates / weight;
    }

    private async Task UpdateImages(Domain.Entities.Dish dish, UpdateRecipeDTO updateDishDTO) 
    {
        if (updateDishDTO.Image != null)
        {
            var filePath = await _imageService.UploadImageAsync(updateDishDTO.Image);

            if (dish.ImageUrl != null)
            {
                await _imageService.DeleteImageAsync(dish.ImageUrl);
            }

            dish.ImageUrl = filePath;
        }
        else if (updateDishDTO.DeleteImageIfNull)
        {
            if (dish.ImageUrl != null)
            {
                await _imageService.DeleteImageAsync(dish.ImageUrl);
            }
        }
    }
}
