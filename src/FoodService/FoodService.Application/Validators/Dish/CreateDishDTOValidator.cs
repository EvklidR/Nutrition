using FluentValidation;
using FoodService.Application.DTOs;
using FoodService.Application.DTOs.Recipe.Requests;
using Microsoft.AspNetCore.Http;

namespace FoodService.Application.Validators
{
    public class CreateDishDTOValidator : AbstractValidator<CreateRecipeDTO>
    {
        public CreateDishDTOValidator(IngredientOfDishDTOValidator ingredientOfDishDTOValidator)
        {
            RuleFor(d => d.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(d => d.AmountOfPortions)
                .GreaterThan(0).WithMessage("Amount should be grater than 0");

            RuleForEach(d => d.Ingredients)
                .SetValidator(ingredientOfDishDTOValidator);
        }
    }
}
