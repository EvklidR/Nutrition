using FoodService.Application.DTOs.Recipe.Responses;

namespace FoodService.Application.UseCases.Queries.Recipe;

public record GetRecipeByIdQuery(Guid RecipeId, Guid UserId) : IQuery<CalculatedRecipeResponse>;
