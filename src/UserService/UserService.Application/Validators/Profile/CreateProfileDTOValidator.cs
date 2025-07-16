using FluentValidation;
using UserService.Application.DTOs.Requests.Profile;

namespace UserService.Application.Validators;

public class CreateProfileDTOValidator : AbstractValidator<CreateProfileDTO>
{
    public CreateProfileDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Weight must be greater than 0.");

        RuleFor(x => x.Height)
            .GreaterThan(0).WithMessage("Height must be greater than 0.");

        RuleFor(x => x.Birthday)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Birthday must be in the past.");

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Gender is invalid.");

        RuleFor(x => x.ActivityLevel)
            .IsInEnum().WithMessage("Activity level is invalid.");
    }
}
