using FluentValidation;
using FoodService.Application.DTOs;
using FoodService.Application.DTOs.DayResult;

namespace FoodService.Application.Validators
{
    public class UpdateDayResultDTOValidator : AbstractValidator<UpdateDayResultDTO>
    {
        public UpdateDayResultDTOValidator()
        {
            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Weight must be grater than 0 if provided")
                .When(x => x.Weight.HasValue);

            RuleFor(x => x.GlassesOfWater)
                .GreaterThan(0).WithMessage("Amount should be grater than 0");
        }
    }
}
