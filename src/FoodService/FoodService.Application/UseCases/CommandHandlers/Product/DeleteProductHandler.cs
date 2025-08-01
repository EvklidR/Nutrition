using FoodService.Application.Exceptions;
using FoodService.Application.UseCases.Commands.Product;
using FoodService.Domain.Entities;
using FoodService.Domain.Interfaces;

namespace FoodService.Application.UseCases.CommandHandlers.Product;

public class DeleteProductHandler : ICommandHandler<DeleteProductCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.ProductId);

        if (product == null)
        {
            throw new NotFound("Product not found");
        }

        if (product.UserId != request.UserId)
        {
            throw new Forbidden("You dont have access to this product");
        }

        var doesDishContains = await _unitOfWork.RecipeRepository.DoesAnyRecipeContainsProductByIdAsync(product.Id);

        if (doesDishContains)
        {
            throw new BadRequest("This product there is in some dish");
        }

        var doesAnyDayResultContainsProduct = await _unitOfWork.DayResultRepository.DoesAnyDayResultContainsFoodByIdAsync(product.Id, aboutProduct: true);

        if (doesAnyDayResultContainsProduct)
        {
            throw new BadRequest("Some day results alredy contain this product");
        }

        _unitOfWork.ProductRepository.Delete(product);

        await _unitOfWork.SaveChangesAsync();
    }
}
