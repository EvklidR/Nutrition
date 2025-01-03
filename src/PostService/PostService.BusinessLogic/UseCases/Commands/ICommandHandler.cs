using MediatR;

namespace PostService.Application.UseCases.Commands
{
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand> 
                                                                        where TCommand : ICommand
    {
        public Task Handle(ICommand request, CancellationToken cancellationToken);

    }

    public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
                                                                                        where TCommand : ICommand<TResult>
    {
        public Task<TResult> Handle(ICommand request, CancellationToken cancellationToken);

    }
}
