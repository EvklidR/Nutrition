using FoodService.Application.Exceptions;
using FoodService.Application.Interfaces;
using FoodService.Domain.Interfaces;
using FoodService.Application.UseCases.Commands.Dishes;

namespace FoodService.Application.UseCases.CommandHandlers.Dishes;

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
        var dish = await _unitOfWork.DishRepository.GetByIdWithRecipeAsync(request.DishId);

        if (dish == null)
        {
            throw new NotFound("Dish not found");
        }

        if (dish.UserId != request.UserId)
        {
            throw new Forbidden("You dont have access to this dish");
        }

        var doesAnyDayResultContainsDish = await _unitOfWork.DayResultRepository.DoesAnyDayResultContainsFoodByIdAsync(dish.Id, aboutProduct: false);

        if (doesAnyDayResultContainsDish)
        {
            throw new BadRequest("Some day results alredy contain this dish");
        }

        _unitOfWork.DishRepository.Delete(dish);

        await _unitOfWork.SaveChangesAsync();

        if (dish.Recipe.ImageUrl != null)
        {
            await _imageService.DeleteImageAsync(dish.Recipe.ImageUrl);
        }
    }
}
