using AutoMapper;
using FoodService.Domain.Interfaces;
using FoodService.Application.Exceptions;
using FoodService.Application.UseCases.Commands.Recipes;
using FoodService.Application.Interfaces;
using FoodService.Application.DTOs.Recipe.Requests;
using FoodService.Domain.Entities;

namespace FoodService.Application.UseCases.CommandHandlers.Recipes;

public class UpdateRecipeHandler : ICommandHandler<UpdateRecipeCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;

    public UpdateRecipeHandler(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageService = imageService;
    }

    public async Task Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe = await _unitOfWork.RecipeRepository.GetFullByIdAsync(request.UpdateRecipeDTO.Id);

        if (recipe == null)
        {
            throw new NotFound("Dish not found");
        }

        if (recipe.Dish.UserId != request.UserId)
        {
            throw new Forbidden("You dont have access to this entity");
        }

        await UpdateImages(recipe, request.UpdateRecipeDTO);

        _mapper.Map(request.UpdateRecipeDTO, recipe);

        await CalculateAndUpdateDish(recipe, request.UpdateRecipeDTO);

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task CalculateAndUpdateDish(Recipe recipe, UpdateRecipeDTO updateRecipeDTO)
    {
        double weight = 0;

        var dish = recipe.Dish;
        dish.Name = updateRecipeDTO.Name;
        dish.Calories = 0;
        dish.Proteins = 0;
        dish.Fats = 0;
        dish.Carbohydrates = 0;

        foreach (var ingredient in recipe.Ingredients)
        {
            var ingredientBD = await _unitOfWork.ProductRepository.GetByIdAsync(ingredient.ProductId);

            if (ingredientBD == null)
            {
                throw new NotFound("Ingredient not found");
            }

            dish.Calories += ingredientBD.Calories * ingredient.WeightInRecipe;
            dish.Proteins += ingredientBD.Proteins * ingredient.WeightInRecipe;
            dish.Fats += ingredientBD.Fats * ingredient.WeightInRecipe;
            dish.Carbohydrates += ingredientBD.Carbohydrates * ingredient.WeightInRecipe;
            weight += ingredient.WeightInRecipe;
        }

        dish.Calories /= weight;
        dish.Fats /= weight;
        dish.Proteins /= weight;
        dish.Carbohydrates /= weight;

        _unitOfWork.DishRepository.Update(dish);
    }

    private async Task UpdateImages(Recipe recipe, UpdateRecipeDTO updateRecipeDTO) 
    {
        if (updateRecipeDTO.Image != null)
        {
            var filePath = await _imageService.UploadImageAsync(updateRecipeDTO.Image);

            if (recipe.ImageUrl != null)
            {
                await _imageService.DeleteImageAsync(recipe.ImageUrl);
            }

            recipe.ImageUrl = filePath;
        }
        else if (updateRecipeDTO.DeleteImageIfNull)
        {
            if (recipe.ImageUrl != null)
            {
                await _imageService.DeleteImageAsync(recipe.ImageUrl);
            }
        }
    }
}
