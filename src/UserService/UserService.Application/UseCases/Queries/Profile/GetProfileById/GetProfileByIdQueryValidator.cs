using FluentValidation;

namespace UserService.Application.UseCases.Queries
{
    public class GetProfileByIdQueryValidator : AbstractValidator<GetProfileByIdQuery>
    {
        public GetProfileByIdQueryValidator()
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
