using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence.Repositories.Interfaces;

namespace TodoApp.Infrastructure.Persistence.Repositories
{
    public class SubTaskRepository : ISubTaskRepository
    {
        private readonly ApplicationDbContext _context;

        public SubTaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task<SubTask?> GetByIdAsync(Guid id)
        {
            return await _context.SubTasks.FindAsync(id);
        }

        public async System.Threading.Tasks.Task<IEnumerable<SubTask>> GetAllAsync()
        {
            return await _context.SubTasks.ToListAsync();
        }

        public async System.Threading.Tasks.Task<IEnumerable<SubTask>> GetByParentTaskIdAsync(Guid parentTaskId)
        {
            return await _context.SubTasks
                .Where(st => st.ParentTaskId == parentTaskId)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<SubTask> AddAsync(SubTask subTask)
        {
            _context.SubTasks.Add(subTask);
            await _context.SaveChangesAsync();
            return subTask;
        }

        public async System.Threading.Tasks.Task<SubTask> UpdateAsync(SubTask subTask)
        {
            _context.SubTasks.Update(subTask);
            await _context.SaveChangesAsync();
            return subTask;
        }

        public async System.Threading.Tasks.Task DeleteAsync(Guid id)
        {
            var subTask = await _context.SubTasks.FindAsync(id);
            if (subTask != null)
            {
                _context.SubTasks.Remove(subTask);
                await _context.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task<bool> ExistsAsync(Guid id)
        {
            return await _context.SubTasks.AnyAsync(st => st.Id == id);
        }
    }
} 