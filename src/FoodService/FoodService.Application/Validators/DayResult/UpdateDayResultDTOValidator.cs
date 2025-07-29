using FluentValidation;
using FoodService.Application.DTOs.DayResult.Requests;

namespace FoodService.Application.Validators
{
    public class UpdateDayResultDTOValidator : AbstractValidator<UpdateDayResultDTO>
    {
        public UpdateDayResultDTOValidator()
        {
            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Weight must be grater than 0 if provided");

            RuleFor(x => x.GlassesOfWater)
                .GreaterThanOrEqualTo(0).WithMessage("Amount should be grater than or equal to 0");
        }
    }
}
