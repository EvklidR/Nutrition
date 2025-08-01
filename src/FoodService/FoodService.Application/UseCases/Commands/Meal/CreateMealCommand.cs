using FoodService.Application.DTOs.Meal.Requests;
using FoodService.Application.DTOs.Meal.Responses;

namespace FoodService.Application.UseCases.Commands.Meal;

public record CreateMealCommand(CreateMealDTO CreateMealDTO, Guid UserId) : ICommand<FullMealResponse>;
