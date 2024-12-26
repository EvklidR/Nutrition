using FluentValidation;

namespace UserService.Application.UseCases.Commands
{
    public class DeleteProfileCommandValidator : AbstractValidator<DeleteProfileCommand>
    {
        public DeleteProfileCommandValidator()
        {
            RuleFor(x => x.profileId)
                .NotEmpty()
                .WithMessage("Profile ID must be provided");

            RuleFor(x => x.userId)
                .NotEmpty()
                .WithMessage("User ID must be provided");
        }
    }
}
