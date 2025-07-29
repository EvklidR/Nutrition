using FoodService.Application.DTOs.Product.Requests;
using FoodService.Application.DTOs.Product.Responses;

namespace FoodService.Application.UseCases.Commands.Product
{
    public record CreateProductCommand(CreateProductDTO CreateProductDTO, Guid UserId) : ICommand<ProductResponse>;
}
