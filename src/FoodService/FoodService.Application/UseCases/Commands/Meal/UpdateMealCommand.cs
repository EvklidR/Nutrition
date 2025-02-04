using MediatR;
using FoodService.Application.DTOs.Meal;

namespace FoodService.Application.UseCases.Commands.Meal
{
    public record UpdateMealCommand(UpdateMealDTO UpdateMealDTO, Guid UserId) : ICommand;
}
