using MediatR;

namespace MealPlanService.Application.UseCases.Commands
{
    public interface ICommand : IRequest
    {
    }

    public interface ICommand<TResult> : IRequest<TResult>
    {
    }
}