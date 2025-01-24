using FluentValidation;
using MealPlanService.Core.Entities;
using MealPlanService.Core.Enums;

namespace MealPlanService.BusinessLogic.Validators
{
    public class MealPlanDayDTOValidator : AbstractValidator<MealPlanDay>
    {
        public MealPlanDayDTOValidator(RecommendationDTOValidator recommendationDTOValidator, NutrientOfDayValidator nutrientOfDayValidator)
        {
            RuleFor(x => x.DayNumber)
                .GreaterThan(0).WithMessage("Day number must be greater than 0");

            RuleFor(x => x.CaloriePercentage)
                .InclusiveBetween(0.1, 1)
                .WithMessage("Calorie percentage must be between 0.1 and 1");

            RuleForEach(x => x.Recommendations)
                .SetValidator(recommendationDTOValidator);

            RuleFor(x => x.NutrientsOfDay)
               .Must(nutrients => nutrients.Select(n => n.NutrientType).Distinct().Count() == 3)
               .WithMessage("Each day must have exactly 3 different nutrient types.")
               .Must(NotExceedOneDefaultNutrient)
               .WithMessage("Each day can have at most one nutrient with a default calculation.")
               .Must(HaveDefaultIfFixedOrPerKg)
               .WithMessage("If there are fixed or per-kg calculations, there must be at least one default calculation.")
               .Must(HaveValidPercentagesIfAllPercent)
               .WithMessage("If all nutrients are calculated as percentages, their sum must equal 1.");

            RuleForEach(x => x.NutrientsOfDay)
               .SetValidator(nutrientOfDayValidator);
        }

        private bool NotExceedOneDefaultNutrient(IEnumerable<NutrientOfDay> nutrients)
        {
            return nutrients.Count(n => n.CalculationType == CalculationType.Bydefault) <= 1;
        }

        private bool HaveDefaultIfFixedOrPerKg(IEnumerable<NutrientOfDay> nutrients)
        {
            bool hasFixedOrPerKg = nutrients.Any(n =>
                n.CalculationType == CalculationType.Fixed ||
                n.CalculationType == CalculationType.PerKg);

            bool hasDefault = nutrients.Any(n => n.CalculationType == CalculationType.Bydefault);

            return !hasFixedOrPerKg || hasDefault;
        }

        private bool HaveValidPercentagesIfAllPercent(IEnumerable<NutrientOfDay> nutrients)
        {
            bool allArePercent = nutrients.All(n => n.CalculationType == CalculationType.Persent);

            if (!allArePercent)
                return true;

            double totalPercentage = nutrients.Sum(n => n.Value ?? 0);

            return totalPercentage == 1;
        }
    }
}
