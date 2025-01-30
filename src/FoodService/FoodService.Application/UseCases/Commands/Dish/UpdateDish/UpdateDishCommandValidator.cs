using FluentValidation;
using FoodService.Application.Validators;

namespace FoodService.Application.UseCases.Commands.Dish.Validators
{
    public class UpdateDishCommandValidator : AbstractValidator<UpdateDishCommand>
    {
        public UpdateDishCommandValidator(UpdateDishDTOValidator updateDishDTOValidator) 
        {
            RuleFor(x => x.UpdateDishDTO)
                .NotEmpty().WithMessage("Updating dish data must not be null.")
                .SetValidator(updateDishDTOValidator);
        }
    }
}
