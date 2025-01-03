using MediatR;

namespace MealPlanService.Application.UseCases.Queries
{
    public interface IQuery<TResult> : IRequest<TResult>
    {

    }
}
