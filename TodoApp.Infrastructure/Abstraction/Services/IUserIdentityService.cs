using System.Security.Claims;
using TodoApp.Application.Common.Result;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Abstraction.Services
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
        Task<Result<Guid>> CreateUserAsync(string email, string password, User domainUser);
        Task<AuthenticationResult> AuthenticateAsync(string email, string password);
        Task<bool> CheckPasswordAsync(Guid userId, string password);
        System.Threading.Tasks.Task AddToRoleAsync(Guid userId, string role);
        Task<bool> IsInRoleAsync(Guid userId, string role);
    }
}
