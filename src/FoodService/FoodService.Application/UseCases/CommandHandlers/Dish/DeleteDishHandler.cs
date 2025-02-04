using FoodService.Application.Exceptions;
using FoodService.Application.Interfaces;
using FoodService.Domain.Interfaces;
using FoodService.Application.UseCases.Commands.Dish;

namespace FoodService.Application.UseCases.CommandHandlers.Dish
{
    public class DeleteDishHandler : ICommandHandler<DeleteDishCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;

        public DeleteDishHandler(IUnitOfWork unitOfWork, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }

        public async Task Handle(DeleteDishCommand request, CancellationToken cancellationToken)
        {
            var dish = await _unitOfWork.DishRepository.GetByIdAsync(request.DishId);

            if (dish == null)
            {
                throw new NotFound("Dish not found");
            }

            if (dish.UserId != request.UserId)
            {
                throw new Forbidden("You dont have access to this dish");
            }

            if (dish.ImageUrl != null)
            {
                await _imageService.DeleteImageAsync(dish.ImageUrl);
            }

            _unitOfWork.DishRepository.Delete(dish);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
