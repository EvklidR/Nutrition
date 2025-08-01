using FoodService.Application.DTOs.Recipe.Requests;

namespace FoodService.Application.UseCases.Commands.Recipes;

public record UpdateRecipeCommand(UpdateRecipeDTO UpdateRecipeDTO, Guid UserId) : ICommand;
