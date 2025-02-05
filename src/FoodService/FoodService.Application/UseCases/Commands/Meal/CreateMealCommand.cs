using FoodService.Application.DTOs.Meal;

namespace FoodService.Application.UseCases.Commands.Meal
{
    public record CreateMealCommand(CreateMealDTO CreateMealDTO, Guid UserId) : ICommand<FullMealDTO>;
}
