using FluentValidation;
using UserService.Application.Validators;

namespace UserService.Application.UseCases.Commands
{
    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator(UpdateProfileDTOValidator updateValidator)
        {
            RuleFor(x => x.profileDto)
                .NotNull()
                .WithMessage("Profile data is required.")
                .SetValidator(updateValidator);
        }
    }
}