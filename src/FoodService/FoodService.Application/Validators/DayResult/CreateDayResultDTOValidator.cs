using FluentValidation;
using FoodService.Application.DTOs;

namespace FoodService.Application.Validators
{
    public class CreateDayResultDTOValidator : AbstractValidator<CreateDayResultDTO>
    {
        public CreateDayResultDTOValidator()
        {
            RuleFor(dr => dr.Date)
                .NotEmpty().WithMessage("Date is required.")
                .Must(d => d < DateOnly.FromDateTime(DateTime.Now))
                .WithMessage("The date must be in the past.");

        }
    }
}
