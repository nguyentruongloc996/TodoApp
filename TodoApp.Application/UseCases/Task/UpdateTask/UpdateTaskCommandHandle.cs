using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.Abstraction.Services;

namespace TodoApp.Application.UseCases.Task.UpdateTask;

public sealed class UpdateTaskCommandHandle(ITaskService taskService) : ICommandHandle<UpdateTaskCommand, TaskDto> {
    public async Task<TaskDto> Handle(UpdateTaskCommand command, CancellationToken cancellationToken) {
        return await taskService.UpdateTaskAsync(command.UpdateTaskDto);
    }
} 