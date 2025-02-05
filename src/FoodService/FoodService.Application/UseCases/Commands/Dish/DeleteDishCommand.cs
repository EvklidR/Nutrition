namespace FoodService.Application.UseCases.Commands.Dish
{
    public record DeleteDishCommand(Guid DishId, Guid UserId) : ICommand;
}
