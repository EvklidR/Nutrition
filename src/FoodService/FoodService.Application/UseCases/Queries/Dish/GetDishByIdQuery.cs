using FoodService.Application.DTOs.Recipe.Responses;

namespace FoodService.Application.UseCases.Queries.Dish
{
    public record GetDishByIdQuery(Guid DishId, Guid UserId) : IQuery<CalculatedRecipeResponse>;
}
