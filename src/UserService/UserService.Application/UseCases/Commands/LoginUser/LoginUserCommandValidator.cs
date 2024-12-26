using FluentValidation;

namespace UserService.Application.UseCases.Commands
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email isn't valid");

            RuleFor(x => x.password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}