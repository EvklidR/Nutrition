using FoodService.Application.UseCases.Commands;
using MediatR;

namespace FoodService.Application.UseCases.CommandHandlers
{
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
                                                                        where TCommand : ICommand
    {

    }

    public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
                                                                                        where TCommand : ICommand<TResult>
    {

    }
}
