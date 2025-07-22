using FoodService.Application.DTOs.Product.Responses;
using FoodService.Domain.Repositories.Models;

namespace FoodService.Application.UseCases.Queries.Product
{
    public record GetProductsQuery(Guid UserId, GetFoodRequestParameters Parameters) : IQuery<ProductsResponse>;
}
