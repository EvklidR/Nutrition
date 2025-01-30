using FluentValidation;
using FoodService.Application.Validators;

namespace FoodService.Application.UseCases.Commands.Product.Validators
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator(CreateProductDTOValidator createProductDTOValidator)
        {
            RuleFor(x => x.CreateProductDTO)
                .NotNull().WithMessage("Ingredient data is required")
                .SetValidator(createProductDTOValidator);
        }
    }
}