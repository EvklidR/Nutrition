using FoodService.Application.Exceptions;
using FoodService.Domain.Interfaces;
using FoodService.Application.UseCases.Commands.Product;

namespace FoodService.Application.UseCases.CommandHandlers.Product
{
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

            var dishes = await _unitOfWork.DishRepository.GetAllAsync(product.UserId);

            var doesDishContains = dishes != null ? dishes.Any(d => d.Ingredients.Any(i => i.ProductId == product.Id)) : false;

            if (doesDishContains)
            {
                throw new BadRequest("This product there is in some dish");
            }

            var dayResults = await _unitOfWork.DayResultRepository.GetAllAsync(product.UserId);

            foreach (var day in dayResults) 
            {
                foreach (var meal in day.Meals)
                { 
                    foreach (var food in meal.Products)
                    {
                        if (food.FoodId == product.Id)
                        {
                            throw new BadRequest("This product there is in some meals");
                        }
                    }
                }
            }

            _unitOfWork.ProductRepository.Delete(product);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
