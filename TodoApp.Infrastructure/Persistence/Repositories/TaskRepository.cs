using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;
using TodoApp.Infrastructure.Persistence.Repositories.Interfaces;

namespace TodoApp.Infrastructure.Persistence.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task<Domain.Entities.Task?> GetByIdAsync(Guid id)
        {
            return await _context.Tasks
                .Include(t => t.SubTasks)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async System.Threading.Tasks.Task<IEnumerable<Domain.Entities.Task>> GetAllAsync()
        {
            return await _context.Tasks
                .Include(t => t.SubTasks)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<IEnumerable<Domain.Entities.Task>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Tasks
                .Include(t => t.SubTasks)
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<IEnumerable<Domain.Entities.Task>> GetByGroupIdAsync(Guid groupId)
        {
            return await _context.Tasks
                .Include(t => t.SubTasks)
                .Where(t => t.GroupId == groupId)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<IEnumerable<Domain.Entities.Task>> GetByStatusAsync(TodoApp.Domain.Enums.TaskStatus status)
        {
            return await _context.Tasks
                .Include(t => t.SubTasks)
                .Where(t => t.Status == status)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<IEnumerable<Domain.Entities.Task>> GetByDueDateAsync(DateTime dueDate)
        {
            return await _context.Tasks
                .Include(t => t.SubTasks)
                .Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == dueDate.Date)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<IEnumerable<Domain.Entities.Task>> GetOverdueTasksAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Tasks
                .Include(t => t.SubTasks)
                .Where(t => t.DueDate.HasValue && t.DueDate.Value < now && t.Status != TodoApp.Domain.Enums.TaskStatus.Completed)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<Domain.Entities.Task> AddAsync(Domain.Entities.Task task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async System.Threading.Tasks.Task<Domain.Entities.Task> UpdateAsync(Domain.Entities.Task task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async System.Threading.Tasks.Task DeleteAsync(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Tasks.AnyAsync(t => t.Id == id);
        }
    }
} 