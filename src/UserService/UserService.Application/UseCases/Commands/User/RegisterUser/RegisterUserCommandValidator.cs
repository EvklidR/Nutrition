using FluentValidation;

namespace UserService.Application.UseCases.Commands
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator() 
        {
            RuleFor(x => x.email)
                .EmailAddress().WithMessage("invalid email");

            RuleFor(x => x.password)
                .NotEmpty().WithMessage("Password must be provided");

            RuleForEach(x => x.url)
                .NotEmpty().WithMessage("Url must be provided");
        }
    }
}
