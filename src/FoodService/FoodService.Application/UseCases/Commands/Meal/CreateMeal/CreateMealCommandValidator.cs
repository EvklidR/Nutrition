using FluentValidation;
using FoodService.Application.Validators.Meal;

namespace FoodService.Application.UseCases.Commands.Meal.Validators
{
    public class CreateMealCommandValidator : AbstractValidator<CreateMealCommand>
    {
        public CreateMealCommandValidator(CreateMealDTOValidator createMealDTOValidator) 
        {
            RuleFor(x => x.CreateMealDTO)
                .NotEmpty().WithMessage("Create data must be not empty")
                .SetValidator(createMealDTOValidator);
        }
    }
}
