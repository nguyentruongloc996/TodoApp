using System;

namespace TodoApp.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;

        // Relation to Application User
        public Guid UserId { get; set; }
        // Note: We'll reference ApplicationUser by interface or keep this as navigation property
    }
}