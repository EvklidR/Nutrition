using MediatR;

namespace FoodService.Application.UseCases.Queries
{
    public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
                                                                                        where TQuery : IQuery<TResult>
    {
    }
}
