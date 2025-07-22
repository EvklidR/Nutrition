using FoodService.Application.DTOs.Meal.Requests;

namespace FoodService.Application.UseCases.Commands.Meal
{
    public record UpdateMealCommand(UpdateMealDTO UpdateMealDTO, Guid UserId) : ICommand;
}
