using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Domain.ValueObjects;
using TodoApp.Application.Abstraction.Repositories;

namespace TodoApp.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.DomainUsers.FindAsync(id);
        }

        public async System.Threading.Tasks.Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.DomainUsers.ToListAsync();
        }

        public async System.Threading.Tasks.Task<IEnumerable<User>> SearchByNameAsync(string name)
        {
            return await _context.DomainUsers
                .Where(u => u.DisplayName.ToLower().Contains(name.ToLower()))
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<User> AddAsync(User user)
        {
            _context.DomainUsers.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async System.Threading.Tasks.Task<User> UpdateAsync(User user)
        {
            _context.DomainUsers.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async System.Threading.Tasks.Task DeleteAsync(Guid id)
        {
            var user = await _context.DomainUsers.FindAsync(id);
            if (user != null)
            {
                _context.DomainUsers.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task<bool> ExistsAsync(Guid id)
        {
            return await _context.DomainUsers.AnyAsync(u => u.Id == id);
        }
    }
} 