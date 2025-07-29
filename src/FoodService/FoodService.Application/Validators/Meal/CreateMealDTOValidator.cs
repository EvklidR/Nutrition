using FluentValidation;
using FoodService.Application.DTOs.Meal.Requests;

namespace FoodService.Application.Validators
{
    public class CreateMealDTOValidator : AbstractValidator<CreateMealDTO>
    {
        public CreateMealDTOValidator(CreateOrUpdateEatenDishDTOValidator createEatenDishDTOValidator, CreateOrUpdateEatenProductDTOValidator createEatenProductDTOValidator)
        {
            RuleForEach(m => m.Products).SetValidator(createEatenProductDTOValidator);
            RuleForEach(m => m.Dishes).SetValidator(createEatenDishDTOValidator);
        }
    }
}
