using MediatR;
using FoodService.Application.DTOs.Dish;

namespace FoodService.Application.UseCases.Commands.Dish
{
    public record UpdateDishCommand(UpdateDishDTO UpdateDishDTO, Guid UserId) : ICommand;
}
