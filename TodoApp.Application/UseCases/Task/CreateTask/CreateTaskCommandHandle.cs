using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.Abstraction.Services;

namespace TodoApp.Application.UseCases.Task.CreateTask;

public sealed class CreateTaskCommandHandle(ITaskService taskService) : ICommandHandle<CreateTaskCommand, TaskDto> {
    public async Task<TaskDto> Handle(CreateTaskCommand command, CancellationToken cancellationToken) {
        return await taskService.CreateTaskAsync(command.CreateTaskDto);
    }
} 