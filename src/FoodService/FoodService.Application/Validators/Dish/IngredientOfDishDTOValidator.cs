using FluentValidation;
using FoodService.Application.DTOs;

namespace FoodService.Application.Validators
{
    public class IngredientOfDishDTOValidator : AbstractValidator<ProductOfDishDTO>
    {
        public IngredientOfDishDTOValidator()
        {
            RuleFor(i => i.IngredientId)
                .GreaterThan(0).WithMessage("IngredientId must be greater than zero.");

            RuleFor(i => i.Weight)
                .GreaterThan(0).WithMessage("Weight must be greater than zero.");
        }
    }
}
