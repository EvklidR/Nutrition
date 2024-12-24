using MediatR;

namespace UserService.Application.UseCases.Commands
{
    public interface ICommand : IRequest
    {
    }

    public interface ICommand<TResult> : IRequest<TResult>
    {
    }
}