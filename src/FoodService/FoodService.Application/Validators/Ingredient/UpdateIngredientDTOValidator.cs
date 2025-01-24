using FluentValidation;
using FoodService.Application.DTOs;

namespace FoodService.Application.Validators
{
    public class UpdateIngredientDTOValidator : AbstractValidator<UpdateProductDTO>
    {
        public UpdateIngredientDTOValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Ingredient ID must be greater than zero.");

            RuleFor(i => i.Name)
                .NotEmpty()
                .WithMessage("Ingredient name is required.");

            RuleFor(i => i.Proteins)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Proteins value must be greater than or equal to zero.");

            RuleFor(i => i.Fats)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Fats value must be greater than or equal to zero.");

            RuleFor(i => i.Carbohydrates)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Carbohydrates value must be greater than or equal to zero.");
        }
    }
}