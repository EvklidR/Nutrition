using FoodService.Domain.Entities;
using FoodService.Domain.Repositories.Models;

namespace FoodService.Domain.Interfaces.Repositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<(IEnumerable<Product>? products, long totalCount)> GetAllAsync(Guid userId, GetFoodRequestParameters parameters);
    }
}
