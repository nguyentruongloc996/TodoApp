namespace TodoApp.Application.Abstraction.Messaging;

public interface ICommandHandle<TCommand> where TCommand : ICommand {
    Task Handle(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandle<TCommand, TResponse> where TCommand : ICommand<TResponse> {
    Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken);
}