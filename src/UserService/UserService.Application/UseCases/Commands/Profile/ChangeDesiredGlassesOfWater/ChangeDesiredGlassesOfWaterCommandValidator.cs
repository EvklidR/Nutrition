using FluentValidation;
using UserService.Application.UseCases.Commands.Profile.IncreaseDesiredGlassesOfWater;

namespace UserService.Application.UseCases.Commands.Profile.ChangeDesiredGlassesOfWater;

public class ChangeDesiredGlassesOfWaterCommandValidator : AbstractValidator<ChangeDesiredGlassesOfWaterCommand>
{
    public ChangeDesiredGlassesOfWaterCommandValidator()
    {
        RuleFor(x => x.DesiredGlassesOfWater)
            .GreaterThanOrEqualTo(0)
                .WithMessage("Amount of glasses should be graeter then or equal to 0");
    }
}
