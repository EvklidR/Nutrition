using FluentValidation;
using FoodService.Application.UseCases.Queries.DayResult;

namespace FoodService.Application.Validators.DayResult
{
    public class GetAllDayResultsQueryValidator : AbstractValidator<GetAllDayResultsQuery>
    {
        public GetAllDayResultsQueryValidator()
        {
            RuleFor(query => query.PeriodParameters)
                .NotEmpty()
                    .WithMessage("PeriodParameters is required");

            RuleFor(query => query.PeriodParameters!.EndDate)
                .GreaterThan(query => query.PeriodParameters!.StartDate)
                    .WithMessage("EndDate must be greater than StartDate")
                .When(query => query.PeriodParameters != null);
        }
    }
}
