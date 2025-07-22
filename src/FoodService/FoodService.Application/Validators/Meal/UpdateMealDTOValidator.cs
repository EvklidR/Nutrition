using FluentValidation;
using FoodService.Application.DTOs.Meal.Requests;

namespace FoodService.Application.Validators
{
    public class UpdateMealDTOValidator : AbstractValidator<UpdateMealDTO>
    {
        public UpdateMealDTOValidator(CreateOrUpdateEatenFoodDTOValidator updateEatenFoodDTOValidator)
        {
            RuleFor(m => m.Name)
                .NotEmpty().WithMessage("Meal name is required");

            RuleForEach(m => m.Products).SetValidator(updateEatenFoodDTOValidator);
            RuleForEach(m => m.Dishes).SetValidator(updateEatenFoodDTOValidator);
        }
    }
}
