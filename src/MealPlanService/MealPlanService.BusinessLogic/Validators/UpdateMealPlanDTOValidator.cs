using FluentValidation;
using MealPlanService.Core.Entities;

namespace MealPlanService.BusinessLogic.Validators
{
    public class UpdateMealPlanDTOValidator : AbstractValidator<MealPlan>
    {
        public UpdateMealPlanDTOValidator(MealPlanDayDTOValidator dayValidator)
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id must be provided");

            RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Name is required")
                    .MaximumLength(50).WithMessage("Name must not exceed 100 characters");

            RuleFor(x => x.Description)
                    .NotEmpty().WithMessage("Description is required")
                    .MaximumLength(2000).WithMessage("Description must not exceed 500 characters");

            RuleFor(x => x.Type)
                    .IsInEnum().WithMessage("Type is not valid");

            RuleFor(x => x.Days)
                    .NotEmpty().WithMessage("At least one day is required")
                    .Must(days => days.Select(d => d.DayNumber).Distinct().Count() == days.Count)
                    .WithMessage("Days must have unique numbers")
                    .Must(days => days.Select(d => d.DayNumber).OrderBy(n => n).SequenceEqual(Enumerable.Range(1, days.Count)))
                    .WithMessage("Days must include all numbers from 1 to the total number of days, in any order");

            RuleForEach(x => x.Days)
                    .SetValidator(dayValidator);
        }
    }
}
