using FoodService.Application.Exceptions;
using FoodService.Application.Interfaces;
using FoodService.Application.UseCases.Commands.Recipes;
using FoodService.Domain.Interfaces;

namespace FoodService.Application.UseCases.CommandHandlers.Recipes;

public class DeleteRecipeHandler : ICommandHandler<DeleteRecipeCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageService _imageService;

    public DeleteRecipeHandler(IUnitOfWork unitOfWork, IImageService imageService)
    {
        _unitOfWork = unitOfWork;
        _imageService = imageService;
    }

    public async Task Handle(DeleteRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe = await _unitOfWork.RecipeRepository.GetFullByIdAsync(request.RecipeId);

        if (recipe == null)
        {
            throw new NotFound("Recipe not found");
        }

        if (recipe.Dish.UserId != request.UserId)
        {
            throw new Forbidden("You dont have access to this recipe");
        }

        var doesAnyDayResultContainsDish = await _unitOfWork.DayResultRepository.DoesAnyDayResultContainsFoodByIdAsync(recipe.Dish.Id, aboutProduct: false);

        if (doesAnyDayResultContainsDish)
        {
            throw new BadRequest("Some day results alredy contain this dish");
        }

        _unitOfWork.RecipeRepository.Delete(recipe);

        await _unitOfWork.SaveChangesAsync();

        if (recipe.ImageUrl != null)
        {
            await _imageService.DeleteImageAsync(recipe.ImageUrl);
        }
    }
}
