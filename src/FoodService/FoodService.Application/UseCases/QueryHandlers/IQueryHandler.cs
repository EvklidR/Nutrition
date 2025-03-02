using FoodService.Application.UseCases.Queries;
using MediatR;

namespace FoodService.Application.UseCases.QueryHandlers
{
    public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
                                                                                        where TQuery : IQuery<TResult>
    {
    }
}
