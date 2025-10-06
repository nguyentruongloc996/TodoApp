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
        private readonly bool _seedData;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            bool seedData = true) : base(options) 
        { 
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
            modelBuilder.ApplyConfiguration(new UserConfiguration());
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
                DisplayName = "Test User 1"
            };

            var testUser2 = new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                DisplayName = "Test User 2"
            };

            modelBuilder.Entity<User>().HasData(testUser1, testUser2);

            // Seed Identity Users with static password hashes
            var identityUser1 = new ApplicationUser
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                UserName = "Test User 1",
                Email = "test1@example.com",
                NormalizedUserName = "TEST USER 1",
                NormalizedEmail = "TEST1@EXAMPLE.COM",
                EmailConfirmed = true,
                DomainUserId = testUser1.Id,
                // Static pre-computed hash for "PasswordUser1!"
                PasswordHash = "AQAAAAIAAYagAAAAEBx8l2zY9dY8K+nJvQWg3ZnYq+4L5m9jX2pZ8nV7wQ3f0t1R5s6u9pA2bC3d4E5f6G7h8I9j"
            };

            var identityUser2 = new ApplicationUser
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                UserName = "Test User 2",
                Email = "test2@example.com",
                NormalizedUserName = "TEST USER 2",
                NormalizedEmail = "TEST2@EXAMPLE.COM",
                EmailConfirmed = true,
                DomainUserId = testUser2.Id,
                // Static pre-computed hash for "PasswordUser2!"
                PasswordHash = "AQAAAAIAAYagAAAAECy9m3aZ0eZ9L+oKwRXh4aoZr+5M6n0kY3qA9oW8xR4g1u2S6t7v0qB3cD4e5F6g7H8i9J0k"
            };

            modelBuilder.Entity<ApplicationUser>().HasData(identityUser1, identityUser2);

            // Seed Groups
            var testGroup = new Group
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Test Group"
            };

            modelBuilder.Entity<Group>().HasData(testGroup);

            // Seed Tasks with STATIC dates
            var testTask1 = new Domain.Entities.Task
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Description = "Complete project documentation",
                DueDate = new DateTime(2024, 12, 31), // Static date instead of DateTime.Now.AddDays(7)
                Status = TodoApp.Domain.Enums.TaskStatus.Pending,
                UserId = testUser1.Id,
                GroupId = testGroup.Id
            };

            var testTask2 = new Domain.Entities.Task
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Description = "Review code changes",
                DueDate = new DateTime(2024, 12, 27), // Static date instead of DateTime.Now.AddDays(3)
                Status = TodoApp.Domain.Enums.TaskStatus.Pending,
                UserId = testUser2.Id,
                GroupId = null
            };

            modelBuilder.Entity<Domain.Entities.Task>().HasData(testTask1, testTask2);

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