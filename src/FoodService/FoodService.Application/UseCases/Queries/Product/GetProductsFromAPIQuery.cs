using FoodService.Application.Models;

namespace FoodService.Application.UseCases.Queries.Product
{
    public record GetProductsFromAPIQuery(string Name) : IQuery<List<ProductResponseFromAPI>?>;
}
