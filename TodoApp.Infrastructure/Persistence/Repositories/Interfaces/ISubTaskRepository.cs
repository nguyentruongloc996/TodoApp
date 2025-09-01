using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Persistence.Repositories.Interfaces
{
    public interface ISubTaskRepository
    {
        System.Threading.Tasks.Task<SubTask?> GetByIdAsync(Guid id);
        System.Threading.Tasks.Task<IEnumerable<SubTask>> GetAllAsync();
        System.Threading.Tasks.Task<IEnumerable<SubTask>> GetByParentTaskIdAsync(Guid parentTaskId);
        System.Threading.Tasks.Task<SubTask> AddAsync(SubTask subTask);
        System.Threading.Tasks.Task<SubTask> UpdateAsync(SubTask subTask);
        System.Threading.Tasks.Task DeleteAsync(Guid id);
        System.Threading.Tasks.Task<bool> ExistsAsync(Guid id);
    }
} 