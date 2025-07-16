using FluentValidation;
using UserService.Application.DTOs.Requests.Profile;

namespace UserService.Application.Validators
{
    public class UpdateProfileDTOValidator : AbstractValidator<UpdateProfileDTO>
    {
        public UpdateProfileDTOValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Name));

            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Weight must be greater than 0.");

            RuleFor(x => x.Height)
                .GreaterThan(0).WithMessage("Height must be greater than 0.");

            RuleFor(x => x.ActivityLevel)
                .IsInEnum().WithMessage("Activity level is invalid.");
        }
    }
}
