using FluentValidation;

namespace UserService.Application.UseCases.Queries
{
    public class CheckUserByIdQueryValidator : AbstractValidator<CheckUserByIdQuery>
    {
        public CheckUserByIdQueryValidator()
        {
            RuleFor(x => x.userId)
                .NotEmpty().WithMessage("Id must be provided");
        }
    }
}
