using Microsoft.AspNetCore.Identity;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Persistence.Auth
{
    public class ApplicationUser : IdentityUser<Guid> 
    {
        // One-way reference: Infrastructure → Domain
        public Guid DomainUserId { get; set; }
        public User DomainUser { get; set; } = null!;
    }
}
