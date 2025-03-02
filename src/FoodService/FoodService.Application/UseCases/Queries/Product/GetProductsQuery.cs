using FoodService.Application.Models;
using FoodService.Domain.Repositories.Models;

namespace FoodService.Application.UseCases.Queries.Product
{
    public record GetProductsQuery(Guid UserId, GetFoodRequestParameters Parameters) : IQuery<ProductsResponse>;
}
