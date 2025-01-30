using FoodService.Application.DTOs.Dish;
using MediatR;

namespace FoodService.Application.UseCases.Queries.Dish
{
    public record GetDishByIdQuery(Guid DishId, Guid UserId) : IQuery<FullDishDishDTO>;
}
