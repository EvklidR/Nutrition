namespace FoodService.Application.UseCases.Commands.Meal
{
    public record DeleteMealCommand(Guid MealId, Guid DayId, Guid UserId) : ICommand;
}
