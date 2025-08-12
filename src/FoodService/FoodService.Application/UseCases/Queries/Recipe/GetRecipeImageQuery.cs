namespace FoodService.Application.UseCases.Queries.Recipes;

public record GetRecipeImageQuery(Guid RecipeId, Guid UserId) : IQuery<Stream>;
