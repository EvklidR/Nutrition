using FoodService.Application.DTOs.Product.Responses;

namespace FoodService.Application.UseCases.Queries.Product
{
    public record GetProductsFromAPIQuery(string Name) : IQuery<List<ProductResponseFromAPI>?>;
}
