using FluentValidation;
using FoodService.Application.Validators;

namespace FoodService.Application.UseCases.Commands.Dish.Validators
{
    public class CreateDishCommandValidator : AbstractValidator<CreateDishCommand>
    {
        public CreateDishCommandValidator(CreateDishDTOValidator createDishDTOValidator) 
        {
            RuleFor(x => x.CreateDishDTO)
                .NotEmpty().WithMessage("Creation dish data must not be null.")
                .SetValidator(createDishDTOValidator);
        }
    }
}
