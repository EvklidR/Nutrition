using MediatR;

namespace FoodService.Application.UseCases.Commands
{
    public interface ICommand : IRequest
    {
    }

    public interface ICommand<TResult> : IRequest<TResult>
    {
    }
}