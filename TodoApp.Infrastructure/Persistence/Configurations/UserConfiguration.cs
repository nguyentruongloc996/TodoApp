using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TodoApp.Domain.Entities;
using TodoApp.Domain.ValueObjects;

namespace TodoApp.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            
            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(u => u.Email)
                .IsRequired()
                .HasConversion(
                    email => email.Value,
                    value => new Email(value))
                .HasMaxLength(255);
                
            builder.Property(u => u.GroupIds)
                .HasConversion(
                    groupIds => string.Join(',', groupIds),
                    value => value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(Guid.Parse)
                        .ToList())
                .Metadata.SetValueComparer(new ValueComparer<List<Guid>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
                
            // Indexes
            builder.HasIndex(u => u.Email)
                .IsUnique();
        }
    }
} 