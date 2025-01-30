using FluentValidation;
using FoodService.Application.Validators;

namespace FoodService.Application.UseCases.Commands.DayResult.Validators
{
    public class CreateDayResultCommandValidator : AbstractValidator<CreateDayResultCommand>
    {
        public CreateDayResultCommandValidator(CreateDayResultDTOValidator createDayResultDTOValidator)
        {
            RuleFor(cmd => cmd.CreateDayResultDTO)
                .NotNull().WithMessage("Creation DayResult data must not be null.")
                .SetValidator(createDayResultDTOValidator);
        }
    }
}
