using FluentValidation;
using FoodService.Application.Validators;

namespace FoodService.Application.UseCases.Commands.Product.Validators
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator(UpdateProductDTOValidator updateProductDTOValidator)
        {
            RuleFor(x => x.UpdateProductDTO)
                .NotEmpty().WithMessage("Ingredient data is required")
                .SetValidator(updateProductDTOValidator);
        }
    }
}