namespace FoodService.Application.UseCases.Commands.Product
{
    public record DeleteProductCommand(Guid ProductId, Guid UserId) : ICommand;
}
