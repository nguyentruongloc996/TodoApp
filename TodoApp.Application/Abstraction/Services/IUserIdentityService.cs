using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Abstraction.Services
{
    public class AuthenticationResult
    {
        public bool IsSuccess { get; set; }
        public Guid? IdentityUserId { get; set; }
        public string? Email { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public interface IUserIdentityService
    {
        Task<Guid> CreateUserAsync(string email, string password, User domainUser);
        Task<AuthenticationResult> AuthenticateAsync(string email, string password);
        Task<bool> CheckPasswordAsync(Guid userId, string password);
        System.Threading.Tasks.Task AddToRoleAsync(Guid userId, string role);
        Task<bool> IsInRoleAsync(Guid userId, string role);
    }
}
