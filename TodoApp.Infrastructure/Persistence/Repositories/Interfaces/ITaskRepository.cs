using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;

namespace TodoApp.Infrastructure.Persistence.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        System.Threading.Tasks.Task<Domain.Entities.Task?> GetByIdAsync(Guid id);
        System.Threading.Tasks.Task<IEnumerable<Domain.Entities.Task>> GetAllAsync();
        System.Threading.Tasks.Task<IEnumerable<Domain.Entities.Task>> GetByUserIdAsync(Guid userId);
        System.Threading.Tasks.Task<IEnumerable<Domain.Entities.Task>> GetByGroupIdAsync(Guid groupId);
        System.Threading.Tasks.Task<IEnumerable<Domain.Entities.Task>> GetByStatusAsync(TodoApp.Domain.Enums.TaskStatus status);
        System.Threading.Tasks.Task<IEnumerable<Domain.Entities.Task>> GetByDueDateAsync(DateTime dueDate);
        System.Threading.Tasks.Task<IEnumerable<Domain.Entities.Task>> GetOverdueTasksAsync();
        System.Threading.Tasks.Task<Domain.Entities.Task> AddAsync(Domain.Entities.Task task);
        System.Threading.Tasks.Task<Domain.Entities.Task> UpdateAsync(Domain.Entities.Task task);
        System.Threading.Tasks.Task DeleteAsync(Guid id);
        System.Threading.Tasks.Task<bool> ExistsAsync(Guid id);
    }
} 