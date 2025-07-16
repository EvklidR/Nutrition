using FluentValidation;

namespace UserService.Application.UseCases.Commands;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("invalid email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password must be provided")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"\d").WithMessage("Password must contain at least one digit")
            .Matches(@"[!@#$%^&*(),.?""':{}|<>]").WithMessage("Password must contain at least one special character");

        RuleForEach(x => x.Url)
            .NotEmpty().WithMessage("Url must be provided");
    }
}
