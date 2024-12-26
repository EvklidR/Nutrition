using FluentValidation;

namespace UserService.Application.UseCases.Commands
{
    public class RevokeTokenCommandValidator : AbstractValidator<RevokeTokenCommand>
    {
        public RevokeTokenCommandValidator()
        {
            RuleFor(x => x.userId)
                .NotEmpty()
                .WithMessage("Id must be greater than 0.");
        }
    }
}
