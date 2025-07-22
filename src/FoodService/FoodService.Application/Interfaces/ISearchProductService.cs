using FoodService.Application.DTOs.Product.Responses;

namespace FoodService.Application.Interfaces
{
    public interface ISearchProductService
    {
        Task<List<ProductResponseFromAPI>?> GetProductsByName(string name);
    }
}
