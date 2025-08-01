using FoodService.Application.DTOs.Recipe.Requests;
using FoodService.Application.DTOs.Recipe.Responses;

namespace FoodService.Application.UseCases.Commands.Recipes;

public record CreateRecipeCommand(CreateRecipeDTO CreateRecipeDTO, Guid UserId) : ICommand<CalculatedRecipeResponse>;
