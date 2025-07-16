using FluentValidation;

namespace UserService.Application.UseCases.Commands
{
    public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator() 
        {
            RuleFor(command => command.Code)
                .NotEmpty()
                    .WithMessage("Confirmation code must not be empty");

            RuleFor(command => command.ChangedEmail)
                .EmailAddress().When(command => command.ChangedEmail != null)
                    .WithMessage("Changed email must be a valid email address");
        }
    }
}
