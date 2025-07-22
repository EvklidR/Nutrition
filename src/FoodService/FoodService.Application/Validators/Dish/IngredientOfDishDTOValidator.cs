using FluentValidation;
using FoodService.Application.DTOs;
using FoodService.Application.DTOs.Recipe.Requests;

namespace FoodService.Application.Validators
{
    public class IngredientOfDishDTOValidator : AbstractValidator<CreateOrUpdateProductOfRecipeDTO>
    {
        public IngredientOfDishDTOValidator()
        {
            RuleFor(i => i.Weight)
                .GreaterThan(0).WithMessage("Weight must be greater than 0");
        }
    }
}
