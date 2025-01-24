using FluentValidation;
using MediatR;
using FoodService.Application.Exceptions;

namespace FoodService.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IValidator<TRequest>? _validator;

        public ValidationBehavior(IValidator<TRequest>? validator)
        {
            _validator = validator;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validator != null)
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResult = await _validator.ValidateAsync(context, cancellationToken);

                if (!validationResult.IsValid)
                {
                    throw new BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                }
            }

            return await next();
        }
    }
}
