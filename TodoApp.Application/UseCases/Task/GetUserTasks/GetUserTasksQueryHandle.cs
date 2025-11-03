using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.Abstraction.Services;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.UseCases.Task.GetUserTasks
{
    internal class GetUserTasksQueryHandle(ICurrentUserService currentUserService,
        ITaskService taskService) : IQueryHandle<GetUserTasksQuery, List<TaskDto>>
    {
        public Task<List<TaskDto>> Handle(GetUserTasksQuery query, CancellationToken cancellationToken)
        {
            var memberId = currentUserService.DomainUserId;
            if (!memberId.HasValue)
            {
                throw new InvalidOperationException("Current user is not authenticated.");
            }

            var tasks = taskService.GetTasksByMemberIdAsync(memberId.Value);

            return tasks;
        }
    }
}
