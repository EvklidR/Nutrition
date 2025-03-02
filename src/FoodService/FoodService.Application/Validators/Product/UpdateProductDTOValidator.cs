using FluentValidation;
using FoodService.Application.DTOs.Product;

namespace FoodService.Application.Validators
{
    public class UpdateProductDTOValidator : AbstractValidator<UpdateProductDTO>
    {
        public UpdateProductDTOValidator()
        {
            RuleFor(i => i.Name)
                .NotEmpty()
                .WithMessage("Ingredient name is required");

            RuleFor(i => i.Proteins)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Proteins value must be greater than or equal to 0");

            RuleFor(i => i.Fats)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Fats value must be greater than or equal to 0");

            RuleFor(i => i.Carbohydrates)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Carbohydrates value must be greater than or equal to 0");
        }
    }
}