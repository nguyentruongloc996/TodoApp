using TodoApp.Infrastructure.Persistence.Auth;

namespace TodoApp.Infrastructure.Persistence.Repositories.Interfaces
{
    public interface IApplicationUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(Guid id);
        Task<ApplicationUser?> GetByIdWithDomainUserAsync(Guid id);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<ApplicationUser?> GetByDomainUserIdAsync(Guid domainUserId);
        Task<ApplicationUser> AddAsync(ApplicationUser user);
        Task<ApplicationUser> UpdateAsync(ApplicationUser user);
        Task DeleteAsync(Guid id);
    }
}
