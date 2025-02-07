using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using PostService.BusinessLogic.Exceptions;

namespace PostService.API.Filters
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
                        throw new BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }.ToString()));
                    }
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }

}