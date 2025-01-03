using MediatR;

namespace UserService.Application.UseCases.Queries
{
    public interface IQuery<TResult> : IRequest<TResult>
    {

    }
}
