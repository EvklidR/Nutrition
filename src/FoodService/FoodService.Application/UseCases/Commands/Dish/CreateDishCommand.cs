using FoodService.Application.DTOs.Dish;

namespace FoodService.Application.UseCases.Commands.Dish
{
    public record CreateDishCommand(CreateDishDTO CreateDishDTO) : ICommand<FullDishDTO>;
}
