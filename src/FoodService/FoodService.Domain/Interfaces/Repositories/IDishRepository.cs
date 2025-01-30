using FoodService.Domain.Entities;
using FoodService.Domain.Repositories.Models;

namespace FoodService.Domain.Interfaces.Repositories
{
    public interface IDishRepository : IBaseRepository<Dish>
    {
        Task<IEnumerable<Dish>?> GetAllAsync(Guid userId, GetFoodRequestParameters requestParameters);
    }
}