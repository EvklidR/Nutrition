using FluentValidation;
using FoodService.Application.DTOs.DayResult.Requests;

namespace FoodService.Application.Validators
{
    public class CreateDayResultDTOValidator : AbstractValidator<CreateDayResultDTO>
    {
        public CreateDayResultDTOValidator()
        {
            RuleFor(dr => dr.Date)
                .NotEmpty().WithMessage("Date is required")
                .Must(d => d < DateOnly.FromDateTime(DateTime.Now))
                .WithMessage("The date must be in the past");

            RuleFor(dr => dr.Weight)
                .GreaterThan(0).WithMessage("Weight must be greater than 0");
        }
    }
}
