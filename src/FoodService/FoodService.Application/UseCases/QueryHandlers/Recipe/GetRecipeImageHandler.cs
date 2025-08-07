using FoodService.Application.Exceptions;
using FoodService.Application.Interfaces;
using FoodService.Application.UseCases.Queries.Recipe;
using FoodService.Domain.Interfaces;

namespace FoodService.Application.UseCases.QueryHandlers.Dish
{
    public class GetDishImageHandler : IQueryHandler<GetRecipeImageQuery, Stream>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;

        public GetDishImageHandler(IUnitOfWork unitOfWork, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }

        public async Task<Stream> Handle(GetRecipeImageQuery request, CancellationToken cancellationToken)
        {
            var recipe = await _unitOfWork.RecipeRepository.GetByIdAsync(request.RecipeId);

            if (recipe == null)
            {
                throw new NotFound("Recipe not found");
            }

            if (recipe.Dish.UserId != request.UserId)
            {
                throw new Forbidden("You have no access to this recipe");
            }

            var imagePath = recipe.ImageUrl;

            if (imagePath == null)
            {
                throw new NotFound("Recipe doesn't have image");
            }

            var imageStream = await _imageService.DownloadImageAsync(imagePath);

            if (imageStream == null)
            {
                throw new NotFound("Failed getting image");
            }

            return imageStream;
        }
    }
}
