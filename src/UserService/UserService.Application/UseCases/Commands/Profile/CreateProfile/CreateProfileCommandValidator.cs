using FluentValidation;
using UserService.Application.Validators;

namespace UserService.Application.UseCases.Commands;

public class CreateProfileCommandValidator : AbstractValidator<CreateProfileCommand>
{
    public CreateProfileCommandValidator(CreateProfileDTOValidator createProfileDTOValidator)
    {
        RuleFor(x => x.ProfileDto)
            .NotNull()
                .WithMessage("Profile data is required.")
            .SetValidator(createProfileDTOValidator);
    }
}