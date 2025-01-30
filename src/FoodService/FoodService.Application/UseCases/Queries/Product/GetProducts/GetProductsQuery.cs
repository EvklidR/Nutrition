using FoodService.Application.DTOs.Product;
using FoodService.Domain.Repositories.Models;
using MediatR;

namespace FoodService.Application.UseCases.Queries.Product
{
    public record GetProductsQuery(Guid UserId, GetFoodRequestParameters Parameters) : IQuery<IEnumerable<ProductDTO>?>;
}
