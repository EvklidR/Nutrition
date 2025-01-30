using FoodService.Application.DTOs.Dish;
using MediatR;

namespace FoodService.Application.UseCases.Commands.Dish
{
    public record CreateDishCommand(CreateDishDTO CreateDishDTO) : ICommand<FullDishDishDTO>;
}
