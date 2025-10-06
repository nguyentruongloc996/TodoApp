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

            builder.HasMany(g => g.Tasks)
                .WithOne(t => t.Group)
                .HasForeignKey(t => t.GroupId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
} 