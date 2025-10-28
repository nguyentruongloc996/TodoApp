using TodoApp.Domain.Entities;

namespace TodoApp.Application.Abstraction.Repositories
{
    public interface IGroupRepository
    {
        System.Threading.Tasks.Task<Group?> GetByIdAsync(Guid id);
        System.Threading.Tasks.Task<IEnumerable<Group>> GetAllAsync();
        System.Threading.Tasks.Task<IEnumerable<Group>> GetByMemberIdAsync(Guid memberId);
        System.Threading.Tasks.Task<Group> AddAsync(Group group);
        System.Threading.Tasks.Task<Group> UpdateAsync(Group group);
        System.Threading.Tasks.Task DeleteAsync(Guid id);
        System.Threading.Tasks.Task<bool> ExistsAsync(Guid id);
        System.Threading.Tasks.Task<bool> IsMemberAsync(Guid groupId, Guid memberId);
    }
}