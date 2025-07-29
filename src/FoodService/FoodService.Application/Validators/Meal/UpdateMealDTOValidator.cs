using FluentValidation;
using FoodService.Application.DTOs.Meal.Requests;

namespace FoodService.Application.Validators
{
    public class UpdateMealDTOValidator : AbstractValidator<UpdateMealDTO>
    {
        public UpdateMealDTOValidator(CreateOrUpdateEatenDishDTOValidator updateEatenDishDTOValidator, CreateOrUpdateEatenProductDTOValidator updateEatenProductDTOValidator)
        {
            RuleForEach(m => m.Products).SetValidator(updateEatenProductDTOValidator);
            RuleForEach(m => m.Dishes).SetValidator(updateEatenDishDTOValidator);
        }
    }
}
