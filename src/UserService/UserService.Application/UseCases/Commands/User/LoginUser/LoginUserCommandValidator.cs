using FluentValidation;

namespace UserService.Application.UseCases.Commands
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("invalid email");

            RuleFor(x => x.password)
                .NotEmpty()
                .WithMessage("Password must be provided");
        }
    }
}
