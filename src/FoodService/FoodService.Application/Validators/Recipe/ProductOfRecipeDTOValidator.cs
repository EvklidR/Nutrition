using FluentValidation;
using FoodService.Application.DTOs.Recipe.Requests;

namespace FoodService.Application.Validators;

public class ProductOfRecipeDTOValidator : AbstractValidator<CreateOrUpdateProductOfRecipeDTO>
{
    public ProductOfRecipeDTOValidator()
    {
        RuleFor(i => i.WeightInRecipe)
            .GreaterThan(0).WithMessage("Weight must be greater than 0");
    }
}
