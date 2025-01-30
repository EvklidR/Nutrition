using FluentValidation;
using FluentValidation.Results;
using FoodService.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FoodService.API.Filters
{
    public class ValidationFilter : IActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var parameter in context.ActionArguments)
            {
                var validatorType = typeof(IValidator<>).MakeGenericType(parameter.Value.GetType());
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var validationResult = validator.Validate(new ValidationContext<object>(parameter.Value));

                    if (!validationResult.IsValid)
                    {
                        context.Result = new BadRequestObjectResult(new
                        {
                            Message = "Validation failed",
                            Errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                        });
                        return;
                    }
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }

}
