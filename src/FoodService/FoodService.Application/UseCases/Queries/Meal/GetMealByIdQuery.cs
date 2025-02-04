using FoodService.Application.DTOs.Meal;

namespace FoodService.Application.UseCases.Queries.Meal
{
    public record GetMealByIdQuery(Guid MealId, Guid DayId, Guid UserId) : IQuery<FullMealDTO>;
}
