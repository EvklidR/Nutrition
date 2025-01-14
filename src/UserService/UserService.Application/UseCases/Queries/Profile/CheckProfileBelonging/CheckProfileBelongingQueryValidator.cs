using FluentValidation;

namespace UserService.Application.UseCases.Queries
{
    public class CheckProfileBelongingQueryValidator : AbstractValidator<CheckProfileBelongingQuery>
    {
        public CheckProfileBelongingQueryValidator()
        {
            RuleFor(x => x.userId)
                .NotEmpty()
                .WithMessage("User plan id should be provided");

            RuleFor(x => x.profileId)
                .NotEmpty()
                .WithMessage("Profile id should be provided");
        }
    }
}
