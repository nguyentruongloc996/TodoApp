using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.UseCases.Task.CompleteTask;
public sealed record CompleteTaskCommand<TaskDto>(Guid TaskId) : ICommand<TaskDto> {}