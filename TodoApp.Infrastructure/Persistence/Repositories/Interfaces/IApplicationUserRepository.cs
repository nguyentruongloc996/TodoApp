using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence.Auth;

namespace TodoApp.Infrastructure.Persistence.Repositories.Interfaces
{
    public interface IApplicationUserRepository
    {
        System.Threading.Tasks.Task<ApplicationUser?> GetByIdAsync(Guid id);
        System.Threading.Tasks.Task<ApplicationUser?> GetByEmailAsync(string email);
        System.Threading.Tasks.Task<IEnumerable<ApplicationUser>> GetAllAsync();
        System.Threading.Tasks.Task<ApplicationUser> AddAsync(ApplicationUser user);
        System.Threading.Tasks.Task<ApplicationUser> UpdateAsync(ApplicationUser user);
        System.Threading.Tasks.Task DeleteAsync(Guid id);
        Task<ApplicationUser?> GetByIdWithDomainUserAsync(Guid id);
    }
}
