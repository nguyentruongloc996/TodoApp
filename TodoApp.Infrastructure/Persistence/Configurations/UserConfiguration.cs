using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TodoApp.Domain.Entities;
using TodoApp.Domain.ValueObjects;

namespace TodoApp.Infrastructure.Persistence.Configurations
{
    public class EmailValueConverter : ValueConverter<Email, string>
    {
        public EmailValueConverter(IDataProtector protector)
            : base(
                email => protector.Protect(email.Value),         // to DB
                encrypted => new Email(protector.Unprotect(encrypted)) // from DB
            )
        { }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public UserConfiguration(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            
            builder.Property(u => u.DisplayName)
                .IsRequired()
                .HasMaxLength(100);

            var protector = _dataProtectionProvider.CreateProtector("DomainUser.Email");
            builder.Property(u => u.Email)
                .IsRequired()
                .HasConversion(new EmailValueConverter(protector))
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