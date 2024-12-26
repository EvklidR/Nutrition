using FluentValidation;

namespace UserService.Application.UseCases.Commands
{
    public class RevokeMealPlanCommandValidator : AbstractValidator<RevokeMealPlanCommand>
    {
        public RevokeMealPlanCommandValidator()
        {
            RuleFor(x => x.profileId)
                .NotEmpty()
                .WithMessage("Profile ID must be provided");

            RuleFor(x => x.userId)
                .NotEmpty()
                .WithMessage("User ID must be provided");
        }
    }
}