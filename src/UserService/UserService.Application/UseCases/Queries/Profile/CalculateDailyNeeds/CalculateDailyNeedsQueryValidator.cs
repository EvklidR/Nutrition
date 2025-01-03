using FluentValidation;

namespace UserService.Application.UseCases.Queries
{
    public class CalculateDailyNeedsQueryValidator : AbstractValidator<CalculateDailyNeedsQuery>
    {
        public CalculateDailyNeedsQueryValidator()
        {
            RuleFor(x => x.profileId)
                .NotEmpty()
                .WithMessage("Profile ID must be provided");
        }
    }
}