using FluentValidation;

namespace UserService.Application.UseCases.Commands
{
    public class ChooseMealPlanCommandValidator : AbstractValidator<ChooseMealPlanCommand>
    {
        public ChooseMealPlanCommandValidator()
        {
            RuleFor(x => x.mealPlanId)
                .NotEmpty()
                .WithMessage("Meal Plan ID must be provided");

            RuleFor(x => x.profileId)
                .NotEmpty()
                .WithMessage("Profile ID must be provided");

            RuleFor(x => x.userId)
                .NotEmpty()
                .WithMessage("User ID must be provided");
        }
    }
}