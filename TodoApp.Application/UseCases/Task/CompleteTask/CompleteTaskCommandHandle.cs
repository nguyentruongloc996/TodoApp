using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.Abstraction.Services;

namespace TodoApp.Application.UseCases.Task.CompleteTask;

public sealed class CompleteTaskCommandHandle(ITaskService taskService) : ICommandHandle<CompleteTaskCommand<TaskDto>, TaskDto> {
    public async Task<TaskDto> Handle(CompleteTaskCommand<TaskDto> command, CancellationToken cancellationToken) {
        return await taskService.CompleteTaskAsync(command.TaskId);
    }
}