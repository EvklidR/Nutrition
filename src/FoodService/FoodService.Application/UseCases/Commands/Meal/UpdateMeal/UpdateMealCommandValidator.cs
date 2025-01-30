using FluentValidation;
using FoodService.Application.Validators.Meal;

namespace FoodService.Application.UseCases.Commands.Meal.Validators
{
    public class UpdateMealCommandValidator : AbstractValidator<UpdateMealCommand>
    {
        public UpdateMealCommandValidator(UpdateMealDTOValidator mealDTOValidator) 
        {
            RuleFor(X => X.UpdateMealDTO)
                .NotEmpty().WithMessage("Upadating data must not be empty")
                .SetValidator(mealDTOValidator);
        }
    }
}
