using FluentValidation;
using FoodService.Application.UseCases.Queries.Product;

namespace FoodService.Application.UseCases.QueryValidators
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
