using FluentValidation;
using FoodService.Application.DTOs;

namespace FoodService.Application.Validators
{
    public class CreateIngredientDTOValidator : AbstractValidator<CreateProductDTO>
    {
        public CreateIngredientDTOValidator()
        {
            RuleFor(x => x.Proteins)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Proteins value must be greater than or equal to zero.");

            RuleFor(x => x.Fats)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Fats value must be greater than or equal to zero.");

            RuleFor(x => x.Carbohydrates)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Carbohydrates value must be greater than or equal to zero.");

            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage("Ingredient name is required.");

            RuleFor(x => x.ProfileId)
                .GreaterThan(0)
                .WithMessage("Profile ID must be greater than zero.");
        }
    }
}