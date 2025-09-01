using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Infrastructure.Persistence.Auth
{
    internal class ApplicationUser : IdentityUser
    {
        public override required string? Email { get; set; }
        public string? Name { get; set; }
        public List<Guid> GroupIds { get; set; } = new();
    }
}
