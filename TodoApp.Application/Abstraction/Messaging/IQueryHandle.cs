namespace TodoApp.Application.Abstraction.Messaging;

public interface IQueryHandle<TQuery> where TQuery : IQuery {
    Task Handle(TQuery query, CancellationToken cancellationToken);
}

public interface IQueryHandle<TQuery, TResponse> where TQuery : IQuery<TResponse> {
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}