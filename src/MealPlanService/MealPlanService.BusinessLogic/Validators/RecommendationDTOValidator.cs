using FluentValidation;
using MealPlanService.Core.Entities;

namespace MealPlanService.BusinessLogic.Validators
{
    public class RecommendationDTOValidator : AbstractValidator<Recommendation>
    {
        public RecommendationDTOValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Recommendation text is required")
                .MaximumLength(250).WithMessage("Recommendation text must not exceed 250 characters");
        }
    }
}
