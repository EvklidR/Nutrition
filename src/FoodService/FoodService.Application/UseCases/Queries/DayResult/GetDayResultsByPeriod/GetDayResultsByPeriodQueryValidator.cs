using FluentValidation;

namespace FoodService.Application.UseCases.Queries.DayResult.Validators
{
    public class GetDayResultsByPeriodQueryValidator : AbstractValidator<GetDayResultsByPeriodQuery>
    {
        public GetDayResultsByPeriodQueryValidator()
        {
            RuleFor(query => query.EndDate)
                .NotEmpty().WithMessage("EndDate is required.")
                .GreaterThan(query => query.StartDate)
                .WithMessage("EndDate must be greater than StartDate.");
        }
    }
}
