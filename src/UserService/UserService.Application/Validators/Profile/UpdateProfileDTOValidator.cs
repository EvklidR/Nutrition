using FluentValidation;
using UserService.Application.DTOs.Requests.ProfileDTOs;

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
                .GreaterThan(0).WithMessage("Weight must be greater than 0.")
                .When(x => x.Weight.HasValue);

            RuleFor(x => x.Height)
                .GreaterThan(0).WithMessage("Height must be greater than 0.")
                .When(x => x.Height.HasValue);

            RuleFor(x => x.ActivityLevel)
                .IsInEnum().WithMessage("Activity level is invalid.")
                .When(x => x.ActivityLevel.HasValue);

            RuleFor(x => x.DesiredGlassesOfWater)
                .GreaterThanOrEqualTo(0).WithMessage("Desired glasses of water must be non-negative.")
                .When(x => x.DesiredGlassesOfWater.HasValue);
        }
    }
}
