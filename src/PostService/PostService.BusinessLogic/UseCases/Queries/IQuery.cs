using MediatR;

namespace PostService.Application.UseCases.Queries
{
    public interface IQuery<TResult> : IRequest<TResult>
    {

    }
}
