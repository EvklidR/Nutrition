namespace FoodService.Application.UseCases.Commands.Dishes;

public record DeleteDishCommand(Guid DishId, Guid UserId) : ICommand;
