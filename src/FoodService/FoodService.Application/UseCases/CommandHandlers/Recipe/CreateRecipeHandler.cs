using AutoMapper;
using FoodService.Domain.Interfaces;
using FoodService.Application.UseCases.Commands.Recipes;
using FoodService.Application.Interfaces;
using FoodService.Application.DTOs.Recipe.Responses;
using FoodService.Domain.Entities;
using FoodService.Application.Exceptions;

namespace FoodService.Application.UseCases.CommandHandlers.Recipes;

public class CreateRecipeHandler : ICommandHandler<CreateRecipeCommand, CalculatedRecipeResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IImageService _imageService;

    public CreateRecipeHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper, IUserService userService, IImageService imageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userService = userService;
        _imageService = imageService;
    }

    public async Task<CalculatedRecipeResponse> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        await _userService.CheckUserByIdAsync(request.UserId);

        var recipe = _mapper.Map<Recipe>(request.CreateRecipeDTO);
        
        await CalculateAndCreateDish(recipe, request);

        var filePath = await _imageService.UploadImageAsync(request.CreateRecipeDTO.Image);

        recipe.ImageUrl = filePath;

        _unitOfWork.RecipeRepository.Add(recipe);

        await _unitOfWork.SaveChangesAsync();

        var recipeEntity = await _unitOfWork.RecipeRepository.GetFullByIdAsync(recipe.Id);

        var recipeDTO = _mapper.Map<CalculatedRecipeResponse>(recipeEntity);

        return recipeDTO;
    }

    private async Task CalculateAndCreateDish(Recipe recipe, CreateRecipeCommand request)
    {
        double weight = 0;
        var dish = new Dish
        {
            Name = request.CreateRecipeDTO.Name,
            Recipe = recipe,
            UserId = request.UserId
        };

        foreach (var ingredient in recipe.Ingredients)
        {
            var ingredientBD = await _unitOfWork.ProductRepository.GetByIdAsync(ingredient.ProductId);

            if (ingredientBD == null)
            {
                throw new NotFound($"Product with id {ingredient.ProductId} not found");
            }

            dish.Calories += ingredientBD.Calories * ingredient.WeightInRecipe;
            dish.Proteins += ingredientBD.Proteins * ingredient.WeightInRecipe;
            dish.Fats += ingredientBD.Fats * ingredient.WeightInRecipe;
            dish.Carbohydrates += ingredientBD.Carbohydrates * ingredient.WeightInRecipe;
            weight += ingredient.WeightInRecipe;
        }

        dish.Calories = Math.Round(dish.Calories / weight, 2);
        dish.Fats = Math.Round(dish.Fats / weight, 2);
        dish.Proteins = Math.Round(dish.Proteins / weight, 2);
        dish.Carbohydrates = Math.Round(dish.Carbohydrates / weight, 2);
        dish.WeightOfPortion = weight / recipe.AmountOfPortions;

        _unitOfWork.DishRepository.Add(dish);

        recipe.Dish = dish;
    }
}
