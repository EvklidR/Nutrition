using MediatR;
using FoodService.Application.DTOs.Product;

namespace FoodService.Application.UseCases.Commands.Product
{
    public record CreateProductCommand(CreateProductDTO CreateProductDTO) : ICommand<ProductDTO>;
}
