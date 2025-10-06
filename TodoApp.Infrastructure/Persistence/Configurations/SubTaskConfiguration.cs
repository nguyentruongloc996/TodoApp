using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Persistence.Configurations
{
    public class SubTaskConfiguration : IEntityTypeConfiguration<SubTask>
    {
        public void Configure(EntityTypeBuilder<SubTask> builder)
        {
            builder.HasKey(st => st.Id);
            
            builder.Property(st => st.Description)
                .IsRequired()
                .HasMaxLength(500);
                
            builder.Property(st => st.Status)
                .IsRequired()
                .HasConversion<string>();
                
            builder.Property(st => st.ParentTaskId)
                .IsRequired();
                
            // Relationships
            builder.HasOne(t => t.ParentTask)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(st => st.ParentTaskId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Indexes
            builder.HasIndex(st => st.ParentTaskId);
            builder.HasIndex(st => st.Status);
        }
    }
} 