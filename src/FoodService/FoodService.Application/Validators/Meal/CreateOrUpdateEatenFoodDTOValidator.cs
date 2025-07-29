using FluentValidation;
using FoodService.Application.DTOs.Meal.Requests;

namespace FoodService.Application.Validators
{
    public class CreateOrUpdateEatenDishDTOValidator : AbstractValidator<CreateOrUpdateEatenDishDTO>
    {
        public CreateOrUpdateEatenDishDTOValidator()
        {
            RuleFor(f => f.AmountOfPortions)
                .GreaterThan(0).WithMessage("Amount of portions must be greater than 0");
        }
    }

    public class CreateOrUpdateEatenProductDTOValidator : AbstractValidator<CreateOrUpdateEatenProductDTO>
    {
        public CreateOrUpdateEatenProductDTOValidator()
        {
            RuleFor(f => f.Weight)
                .GreaterThan(0).WithMessage("Weight must be greater than 0");
        }
    }
}
