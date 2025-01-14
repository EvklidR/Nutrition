using FluentValidation;
using MealPlanService.BusinessLogic.DTOs;

namespace MealPlanService.BusinessLogic.Validators
{
    public class UsersMealPlanDTOValidator : AbstractValidator<ProfileMealPlanDTO>
    {
        public UsersMealPlanDTOValidator()
        {
            RuleFor(x => x.MealPlanId)
                .NotEmpty().WithMessage("MealPlan Id is required");

            RuleFor(x => x.ProfileId)
                .NotEmpty().WithMessage("Profile Id is required");
        }
    }
}
