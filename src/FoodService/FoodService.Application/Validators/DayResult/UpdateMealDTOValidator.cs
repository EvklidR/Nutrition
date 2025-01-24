using FluentValidation;
using FoodService.Application.DTOs;

namespace FoodService.Application.Validators
{
    public class UpdateMealDTOValidator : AbstractValidator<UpdateMealDTO>
    {
        public UpdateMealDTOValidator(CreateEatenFoodDTOValidator createEatenFoodDTOValidator)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id should be grater than 0");
            RuleFor(x => x.DayResultId)
                .GreaterThan(0).WithMessage("Day id should be garter than 0");
            RuleFor(m => m.Name)
                .NotEmpty().WithMessage("Meal name is required.");

            RuleForEach(m => m.Foods).SetValidator(createEatenFoodDTOValidator);
        }
    }
}
