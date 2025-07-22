using FoodService.Application.DTOs.Recipe.Requests;

namespace FoodService.Application.UseCases.Commands.Dish
{
    public record UpdateDishCommand(UpdateRecipeDTO UpdateDishDTO, Guid UserId) : ICommand;
}
