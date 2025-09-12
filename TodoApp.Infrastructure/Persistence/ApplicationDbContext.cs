using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Domain.ValueObjects;
using TodoApp.Infrastructure.Persistence.Auth;
using TodoApp.Infrastructure.Persistence.Configurations;

namespace TodoApp.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly bool _seedData;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IDataProtectionProvider dataProtectionProvider, 
            bool seedData = true) : base(options) 
        { 
            _dataProtectionProvider = dataProtectionProvider;
            _seedData = seedData;
        }

        public DbSet<Domain.Entities.Task> Tasks { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<User> DomainUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new TaskConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration(_dataProtectionProvider));
            modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
            modelBuilder.ApplyConfiguration(new GroupConfiguration());
            modelBuilder.ApplyConfiguration(new SubTaskConfiguration());

            // Only seed data if explicitly requested
            if (_seedData)
            {
                SeedData(modelBuilder);
            }
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Domain Users first
            var testUser1 = new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Email = new Email("test1@example.com"),
                DisplayName = "Test User 1",
                GroupIds = new List<Guid>()
            };

            var testUser2 = new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Email = new Email("test2@example.com"),
                DisplayName = "Test User 2",
                GroupIds = new List<Guid>()
            };

            modelBuilder.Entity<User>().HasData(testUser1, testUser2);

            // Then seed Identity Users that reference Domain Users
            var hasher = new PasswordHasher<ApplicationUser>();
            
            var identityUser1 = new ApplicationUser
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                UserName = testUser1.Email.Value,
                Email = testUser1.Email.Value,
                NormalizedUserName = testUser1.Email.Value.ToUpperInvariant(),
                NormalizedEmail = testUser1.Email.Value.ToUpperInvariant(),
                EmailConfirmed = true,
                DomainUserId = testUser1.Id
            };
            identityUser1.PasswordHash = hasher.HashPassword(identityUser1, "PasswordUser1!");

            var identityUser2 = new ApplicationUser
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                UserName = testUser2.Email.Value,
                Email = testUser2.Email.Value,
                NormalizedUserName = testUser2.Email.Value.ToUpperInvariant(),
                NormalizedEmail = testUser2.Email.Value.ToUpperInvariant(),
                EmailConfirmed = true,
                DomainUserId = testUser2.Id
            };
            identityUser2.PasswordHash = hasher.HashPassword(identityUser2, "PasswordUser2!");

            modelBuilder.Entity<ApplicationUser>().HasData(identityUser1, identityUser2);

            // Seed Groups
            var testGroup = new Group
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Test Group",
                MemberIds = new List<Guid> { testUser1.Id, testUser2.Id },
                TaskIds = new List<Guid>()
            };

            modelBuilder.Entity<Group>().HasData(testGroup);

            // Update users with group membership
            testUser1.GroupIds.Add(testGroup.Id);
            testUser2.GroupIds.Add(testGroup.Id);

            // Seed Tasks
            var testTask1 = new Domain.Entities.Task
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Description = "Complete project documentation",
                DueDate = DateTime.Now.AddDays(7),
                Status = TodoApp.Domain.Enums.TaskStatus.Pending,
                UserId = testUser1.Id,
                GroupId = testGroup.Id,
                SubTasks = new List<SubTask>()
            };

            var testTask2 = new Domain.Entities.Task
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Description = "Review code changes",
                DueDate = DateTime.Now.AddDays(3),
                Status = TodoApp.Domain.Enums.TaskStatus.Pending,
                UserId = testUser2.Id,
                GroupId = null,
                SubTasks = new List<SubTask>()
            };

            modelBuilder.Entity<Domain.Entities.Task>().HasData(testTask1, testTask2);

            // Update group with task
            testGroup.TaskIds.Add(testTask1.Id);

            // Seed SubTasks
            var subTask1 = new SubTask
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Description = "Write API documentation",
                Status = TodoApp.Domain.Enums.TaskStatus.Pending,
                ParentTaskId = testTask1.Id
            };

            var subTask2 = new SubTask
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                Description = "Create user guide",
                Status = TodoApp.Domain.Enums.TaskStatus.Completed,
                ParentTaskId = testTask1.Id
            };

            modelBuilder.Entity<SubTask>().HasData(subTask1, subTask2);
        }
    }
}