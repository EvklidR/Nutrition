using FluentValidation;

namespace UserService.Application.UseCases.Commands
{
    public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator() 
        {
            RuleFor(command => command.userId)
                .NotEmpty().WithMessage("User ID must not be empty");

            RuleFor(command => command.code)
                .NotEmpty().WithMessage("Confirmation code must not be empty");

            RuleFor(command => command.changedEmail)
                .EmailAddress().When(command => command.changedEmail != null)
                .WithMessage("Changed email must be a valid email address");
        }
    }
}
