using FoodService.Application.Models;
using FoodService.Domain.Repositories.Models;

namespace FoodService.Application.UseCases.Queries.Dish
{
    public record GetDishesQuery(Guid UserId, GetFoodRequestParameters Parameters) 
        : IQuery<DishesResponse>;
}
