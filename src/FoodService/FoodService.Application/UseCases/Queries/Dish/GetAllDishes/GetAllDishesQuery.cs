using FoodService.Application.DTOs.Dish;
using FoodService.Domain.Repositories.Models;
using MediatR;

namespace FoodService.Application.UseCases.Queries.Dish
{
    public record GetAllDishesQuery(Guid UserId, GetFoodRequestParameters Parameters) 
        : IQuery<IEnumerable<BriefDishDishDTO>?>;
}
