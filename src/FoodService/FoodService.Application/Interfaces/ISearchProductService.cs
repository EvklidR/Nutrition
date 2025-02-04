using FoodService.Application.Models;

namespace FoodService.Application.Interfaces
{
    public interface ISearchProductService
    {
        Task<List<ProductResponseFromAPI>?> GetProductsByName(string name);
    }
}
