using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.UseCases.Task.UpdateTask;
public sealed record UpdateTaskCommand(UpdateTaskDto UpdateTaskDto) : ICommand<TaskDto> {} 