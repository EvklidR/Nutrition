using FluentValidation;
using MealPlanService.Core.Entities;

namespace MealPlanService.BusinessLogic.Validators
{
    public class NutrientOfDayValidator : AbstractValidator<NutrientOfDay>
    {
        public NutrientOfDayValidator()
        {
            RuleFor(n => n.NutrientType)
                .IsInEnum().WithMessage("Invalid nutrient type.");

            RuleFor(n => n.CalculationType)
                .IsInEnum().WithMessage("Invalid calculation type.");

            RuleFor(n => n.Value)
                .GreaterThanOrEqualTo(0).When(n => n.Value.HasValue)
                .WithMessage("Nutrient value must be greater than or equal to 0.");

        }
    }
}
