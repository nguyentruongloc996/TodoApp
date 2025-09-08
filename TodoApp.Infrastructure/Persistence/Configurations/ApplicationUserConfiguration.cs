using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Infrastructure.Persistence.Auth;

namespace TodoApp.Infrastructure.Persistence.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Configure the one-to-one relationship
            builder.HasOne(au => au.DomainUser)
                .WithMany() // No navigation property on User side
                .HasForeignKey(au => au.DomainUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(au => au.DomainUserId)
                .IsUnique();
        }
    }
}