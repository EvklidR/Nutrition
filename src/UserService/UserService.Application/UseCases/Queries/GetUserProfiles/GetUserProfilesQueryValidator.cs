using FluentValidation;

namespace UserService.Application.UseCases.Queries
{
    public class GetUserProfilesQueryValidator : AbstractValidator<GetUserProfilesQuery>
    {
        public GetUserProfilesQueryValidator() 
        {
            RuleFor(x => x.userId)
                .NotEmpty()
                .WithMessage("User ID must be provided");
        }
    }
}
