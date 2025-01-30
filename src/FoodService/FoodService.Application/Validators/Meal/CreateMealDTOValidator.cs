using FluentValidation;
using FoodService.Application.DTOs.Meal;

namespace FoodService.Application.Validators
{
    public class CreateMealDTOValidator : AbstractValidator<CreateMealDTO>
    {
        public CreateMealDTOValidator(CreateOrUpdateEatenFoodDTOValidator сreateEatenFoodDTOValidator)
        {
            RuleFor(m => m.Name)
                .NotEmpty().WithMessage("Meal name is required");

            RuleForEach(m => m.Products).SetValidator(сreateEatenFoodDTOValidator);
            RuleForEach(m => m.Dishes).SetValidator(сreateEatenFoodDTOValidator);

        }
    }
}
