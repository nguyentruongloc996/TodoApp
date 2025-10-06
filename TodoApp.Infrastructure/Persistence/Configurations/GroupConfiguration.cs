using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Persistence.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.HasKey(g => g.Id);
            
            builder.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Configure Many-to-Many relationship with Users
            builder.HasMany(g => g.Members)
                .WithMany(u => u.Groups)
                .UsingEntity<Dictionary<string, object>>(
                    "GroupMembers",
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j => j.HasOne<Group>().WithMany().HasForeignKey("GroupId"));

            // Configure One-to-Many relationship with Tasks
            builder.HasMany(g => g.Tasks)
                .WithOne(t => t.Group)
                .HasForeignKey(t => t.GroupId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}