using FoodService.Application.DTOs.Product.Requests;

namespace FoodService.Application.UseCases.Commands.Product
{
    public record UpdateProductCommand(UpdateProductDTO UpdateProductDTO, Guid UserId) : ICommand;
}
