using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence.Auth;

namespace TodoApp.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(rt => rt.Id);
            
            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);
                
            builder.Property(rt => rt.Created)
                .IsRequired();
                
            builder.Property(rt => rt.Expires)
                .IsRequired();
                
            builder.Property(rt => rt.UserId)
                .IsRequired();

            // Configure foreign key relationship to ApplicationUser
            // This is acceptable because it's Infrastructure configuration
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add index for performance
            builder.HasIndex(rt => rt.UserId);
            builder.HasIndex(rt => rt.Token).IsUnique();
        }
    }
}