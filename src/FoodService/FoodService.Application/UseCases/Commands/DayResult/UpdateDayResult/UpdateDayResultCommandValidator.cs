using FluentValidation;
using FoodService.Application.Validators;

namespace FoodService.Application.UseCases.Commands.DayResult.Validators
{
    public class UpdateDayResultCommandValidator : AbstractValidator<UpdateDayResultCommand>
    {
        public UpdateDayResultCommandValidator(UpdateDayResultDTOValidator updateDayResultDTOValidator) 
        {
            RuleFor(x => x.UpdateDayResultDTO)
                .NotEmpty().WithMessage("Updating DayResult data must not be null.")
                .SetValidator(updateDayResultDTOValidator);
        }
    }
}
