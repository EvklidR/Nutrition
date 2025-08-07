using AutoMapper;
using FoodService.Domain.Interfaces;
using FoodService.Application.Exceptions;
using FoodService.Application.DTOs.Recipe.Responses;
using FoodService.Application.UseCases.Queries.Recipe;

namespace FoodService.Application.UseCases.QueryHandlers.Recipe
{
    public class GetRecipeByIdHandler : IQueryHandler<GetRecipeByIdQuery, CalculatedRecipeResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetRecipeByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CalculatedRecipeResponse> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
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

            var recipeDTO = _mapper.Map<CalculatedRecipeResponse>(recipe);

            return recipeDTO;
        }
    }
}
