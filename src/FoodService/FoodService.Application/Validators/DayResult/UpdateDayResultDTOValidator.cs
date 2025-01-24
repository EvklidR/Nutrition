using FluentValidation;
using FoodService.Application.DTOs;

namespace FoodService.Application.Validators
{
    public class UpdateDayResultDTOValidator : AbstractValidator<UpdateDayResultDTO>
    {
        public UpdateDayResultDTOValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Day id must be grater than 0");
            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Weight must be grater than 0 if provided")
                .When(x => x.Weight.HasValue);
            RuleFor(x => x.GlassesOfWater)
                .GreaterThan(0).WithMessage("Amount should be grater than 0");
        }
    }
}
