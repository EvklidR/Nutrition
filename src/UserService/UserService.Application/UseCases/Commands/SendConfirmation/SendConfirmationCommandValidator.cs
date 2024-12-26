using FluentValidation;

namespace UserService.Application.UseCases.Commands
{
    public class SendConfirmationCommandValidator : AbstractValidator<SendConfirmationCommand>
    {
        public SendConfirmationCommandValidator() 
        {
            RuleFor(x => x.email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");
        }
    }
}
