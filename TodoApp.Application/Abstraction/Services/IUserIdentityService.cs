using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Application.Abstraction.Services
{
    public interface IUserIdentityService
    {
        Task<Guid> CreateUserAsync(string email, string password);
        Task<bool> CheckPasswordAsync(Guid userId, string password);
        Task AddToRoleAsync(Guid userId, string role);
        Task<bool> IsInRoleAsync(Guid userId, string role);
    }
}
