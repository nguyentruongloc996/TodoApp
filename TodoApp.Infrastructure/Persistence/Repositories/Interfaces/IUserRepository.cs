using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Persistence.Repositories.Interfaces
{
    public interface IUserRepository
    {
        System.Threading.Tasks.Task<User?> GetByIdAsync(Guid id);
        System.Threading.Tasks.Task<User?> GetByEmailAsync(string email);
        System.Threading.Tasks.Task<IEnumerable<User>> GetAllAsync();
        System.Threading.Tasks.Task<IEnumerable<User>> SearchByNameAsync(string name);
        System.Threading.Tasks.Task<User> AddAsync(User user);
        System.Threading.Tasks.Task<User> UpdateAsync(User user);
        System.Threading.Tasks.Task DeleteAsync(Guid id);
        System.Threading.Tasks.Task<bool> ExistsAsync(Guid id);
        System.Threading.Tasks.Task<bool> ExistsByEmailAsync(string email);
    }
}