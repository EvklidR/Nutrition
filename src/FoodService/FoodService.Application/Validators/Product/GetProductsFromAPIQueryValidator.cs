using FluentValidation;
using FoodService.Application.UseCases.Queries.Product;

namespace FoodService.Application.Validators.Product
{
    public class GetProductsFromAPIQueryValidator : AbstractValidator<GetProductsFromAPIQuery>
    {
        public GetProductsFromAPIQueryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name of product must be not empty");
        }
    }
}
