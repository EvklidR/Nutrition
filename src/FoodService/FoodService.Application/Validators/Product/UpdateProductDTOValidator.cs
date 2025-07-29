using FluentValidation;
using FoodService.Application.DTOs.Product.Requests;

namespace FoodService.Application.Validators
{
    public class UpdateProductDTOValidator : AbstractValidator<UpdateProductDTO>
    {
        public UpdateProductDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Ingredient name is required");

            RuleFor(x => x.Proteins)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Proteins value must be greater than or equal to 0");

            RuleFor(x => x.Fats)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Fats value must be greater than or equal to 0");

            RuleFor(x => x.Carbohydrates)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Carbohydrates value must be greater than or equal to 0");
        }
    }
}