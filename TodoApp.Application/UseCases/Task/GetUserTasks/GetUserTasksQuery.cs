using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.UseCases.Task.GetUserTasks
{
    public record GetUserTasksQuery : IQuery<List<TaskDto>> { }
}
