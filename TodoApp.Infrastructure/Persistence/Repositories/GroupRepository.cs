using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence.Repositories.Interfaces;

namespace TodoApp.Infrastructure.Persistence.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public GroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task<Group?> GetByIdAsync(Guid id)
        {
            return await _context.Groups.FindAsync(id);
        }

        public async System.Threading.Tasks.Task<IEnumerable<Group>> GetAllAsync()
        {
            return await _context.Groups.ToListAsync();
        }

        public async System.Threading.Tasks.Task<IEnumerable<Group>> GetByMemberIdAsync(Guid memberId)
        {
            return await _context.Groups
                .Where(g => g.Members.Any(mem => mem.Id == memberId))
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<Group> AddAsync(Group group)
        {
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            return group;
        }

        public async System.Threading.Tasks.Task<Group> UpdateAsync(Group group)
        {
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
            return group;
        }

        public async System.Threading.Tasks.Task DeleteAsync(Guid id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group != null)
            {
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Groups.AnyAsync(g => g.Id == id);
        }

        public async System.Threading.Tasks.Task<bool> IsMemberAsync(Guid groupId, Guid memberId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            return group?.Members.Any(member => member.Id == memberId) ?? false;
        }
    }
} 