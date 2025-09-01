using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Persistence.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<Domain.Entities.Task>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Task> builder)
        {
            builder.HasKey(t => t.Id);
            
            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(1000);
                
            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>();
                
            builder.Property(t => t.DueDate)
                .IsRequired(false);
                
            // Ignore complex value objects for now - they can be implemented later
            builder.Ignore(t => t.Reminder);
            builder.Ignore(t => t.RepeatPattern);
                
            builder.Property(t => t.GroupId)
                .IsRequired(false);
                
            builder.Property(t => t.UserId)
                .IsRequired();
                
            // Relationships
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasOne<Group>()
                .WithMany()
                .HasForeignKey(t => t.GroupId)
                .OnDelete(DeleteBehavior.SetNull);
                
            builder.HasMany(t => t.SubTasks)
                .WithOne()
                .HasForeignKey(st => st.ParentTaskId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Indexes
            builder.HasIndex(t => t.UserId);
            builder.HasIndex(t => t.GroupId);
            builder.HasIndex(t => t.Status);
            builder.HasIndex(t => t.DueDate);
        }
    }
} 