using FoodService.Application.DTOs.Dish;

namespace FoodService.Application.UseCases.Queries.Dish
{
    public record GetDishByIdQuery(Guid DishId, Guid UserId) : IQuery<FullDishDTO>;
}
