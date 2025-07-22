using FoodService.Application.DTOs.Recipe.Requests;
using FoodService.Application.DTOs.Recipe.Responses;

namespace FoodService.Application.UseCases.Commands.Dish
{
    public record CreateDishCommand(CreateRecipeDTO CreateDishDTO) : ICommand<CalculatedRecipeResponse>;
}
