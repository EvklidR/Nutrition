using FluentValidation;
using FoodService.Application.DTOs.Meal.Requests;

namespace FoodService.Application.Validators
{
    public class CreateOrUpdateEatenFoodDTOValidator : AbstractValidator<CreateOrUpdateEatenFoodDTO>
    {
        public CreateOrUpdateEatenFoodDTOValidator()
        {
            RuleFor(f => f.Weight)
                .GreaterThan(0).WithMessage("Weight must be greater than 0");
        }
    }
}
