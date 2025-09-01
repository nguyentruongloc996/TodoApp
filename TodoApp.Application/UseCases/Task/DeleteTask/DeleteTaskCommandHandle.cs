using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.Abstraction.Services;

namespace TodoApp.Application.UseCases.Task.DeleteTask;

public sealed class DeleteTaskCommandHandle(ITaskService taskService) : ICommandHandle<DeleteTaskCommand, bool> {
    public async Task<bool> Handle(DeleteTaskCommand command, CancellationToken cancellationToken) {
        return await taskService.DeleteTaskAsync(command.TaskId);
    }
} 