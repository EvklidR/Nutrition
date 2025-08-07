namespace FoodService.Application.UseCases.Queries.Recipe;

public record GetRecipeImageQuery(Guid RecipeId, Guid UserId) : IQuery<Stream>;
