using MediatR;

namespace FoodService.Application.UseCases.Queries
{
    public interface IQuery<TResult> : IRequest<TResult>
    {

    }
}
