﻿using FluentValidation;
using FoodService.Application.DTOs;
using FoodService.Application.DTOs.Dish;

namespace FoodService.Application.Validators
{
    public class UpdateDishDTOValidator : AbstractValidator<UpdateDishDTO>
    {
        public UpdateDishDTOValidator(IngredientOfDishDTOValidator ingredientOfDishDTOValidator)
        {
            RuleFor(d => d.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(d => d.AmountOfPortions)
                .GreaterThan(0).WithMessage("Amount should be grater than 0");

            RuleForEach(d => d.Ingredients)
                .SetValidator(ingredientOfDishDTOValidator);
        }
    }
}
