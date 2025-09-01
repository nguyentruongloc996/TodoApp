using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.UseCases.Task.CreateTask;
public sealed record CreateTaskCommand(CreateTaskDto CreateTaskDto) : ICommand<TaskDto> {} 