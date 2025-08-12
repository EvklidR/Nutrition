namespace FoodService.Application.UseCases.Commands.Recipes;

public record DeleteRecipeCommand(Guid RecipeId, Guid UserId) : ICommand;
