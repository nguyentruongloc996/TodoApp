using TodoApp.Application.Abstraction.Messaging;

namespace TodoApp.Application.UseCases.Task.DeleteTask;
public sealed record DeleteTaskCommand(Guid TaskId) : ICommand<bool> {} 