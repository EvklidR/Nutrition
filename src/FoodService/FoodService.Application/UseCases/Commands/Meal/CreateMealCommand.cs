using FoodService.Application.DTOs.Meal;
using MediatR;

namespace FoodService.Application.UseCases.Commands.Meal
{
    public record CreateMealCommand(CreateMealDTO CreateMealDTO, Guid UserId) : ICommand<FullMealDTO>;
}
