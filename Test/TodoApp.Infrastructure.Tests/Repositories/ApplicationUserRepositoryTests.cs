using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Domain.ValueObjects;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Persistence.Auth;
using TodoApp.Infrastructure.Persistence.Repositories;
using Xunit;

namespace TodoApp.Infrastructure.Tests.Repositories
{
    public class ApplicationUserRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly IDataProtectionProvider _dataProtectionProvider = DataProtectionProvider.Create("ApplicationUserRepositoryTests");

        public ApplicationUserRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private ApplicationDbContext CreateContext()
        {
            // Pass seedData: false to disable seed data
            var context = new ApplicationDbContext(_options, _dataProtectionProvider, seedData: false);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByIdAsync_ShouldReturnApplicationUser_WhenUserExists()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ApplicationUserRepository(context);
            
            var domainUser = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email("test@example.com"),
                DisplayName = "Test User"
            };

            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "test@example.com",
                Email = "test@example.com",
                DomainUserId = domainUser.Id,
                DomainUser = domainUser
            };

            context.DomainUsers.Add(domainUser);
            context.Users.Add(applicationUser);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(applicationUser.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(applicationUser.Id, result.Id);
            Assert.Equal(applicationUser.Email, result.Email);
            Assert.Equal(applicationUser.UserName, result.UserName);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ApplicationUserRepository(context);
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await repository.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByIdWithDomainUserAsync_ShouldReturnApplicationUserWithDomainUser_WhenUserExists()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ApplicationUserRepository(context);
            
            var domainUser = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email("test@example.com"),
                DisplayName = "Test User"
            };

            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "test@example.com",
                Email = "test@example.com",
                DomainUserId = domainUser.Id,
                DomainUser = domainUser
            };

            context.DomainUsers.Add(domainUser);
            context.Users.Add(applicationUser);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdWithDomainUserAsync(applicationUser.Id);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.DomainUser);
            Assert.Equal(applicationUser.Id, result.Id);
            Assert.Equal(domainUser.Id, result.DomainUser.Id);
            Assert.Equal(domainUser.Email.Value, result.DomainUser.Email.Value);
            Assert.Equal(domainUser.DisplayName, result.DomainUser.DisplayName);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByIdWithDomainUserAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ApplicationUserRepository(context);
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await repository.GetByIdWithDomainUserAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByEmailAsync_ShouldReturnApplicationUser_WhenUserExists()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ApplicationUserRepository(context);
            var email = "test@example.com";
            
            var domainUser = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email(email),
                DisplayName = "Test User"
            };

            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                DomainUserId = domainUser.Id,
                DomainUser = domainUser
            };

            context.DomainUsers.Add(domainUser);
            context.Users.Add(applicationUser);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(applicationUser.Id, result.Id);
            Assert.Equal(email, result.Email);
            Assert.Equal(email, result.UserName);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ApplicationUserRepository(context);
            var nonExistentEmail = "nonexistent@example.com";

            // Act
            var result = await repository.GetByEmailAsync(nonExistentEmail);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetAllAsync_ShouldReturnAllApplicationUsers()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ApplicationUserRepository(context);
            
            var domainUser1 = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email("user1@example.com"),
                DisplayName = "User 1"
            };

            var domainUser2 = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email("user2@example.com"),
                DisplayName = "User 2"
            };

            var applicationUser1 = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "user1@example.com",
                Email = "user1@example.com",
                DomainUserId = domainUser1.Id,
                DomainUser = domainUser1
            };

            var applicationUser2 = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "user2@example.com",
                Email = "user2@example.com",
                DomainUserId = domainUser2.Id,
                DomainUser = domainUser2
            };

            context.DomainUsers.AddRange(domainUser1, domainUser2);
            context.Users.AddRange(applicationUser1, applicationUser2);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, u => u.Id == applicationUser1.Id);
            Assert.Contains(result, u => u.Id == applicationUser2.Id);
        }

        [Fact]
        public async System.Threading.Tasks.Task AddAsync_ShouldAddApplicationUserToDatabase()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ApplicationUserRepository(context);
            
            var domainUser = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email("test@example.com"),
                DisplayName = "Test User"
            };

            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "test@example.com",
                Email = "test@example.com",
                DomainUserId = domainUser.Id,
                DomainUser = domainUser
            };

            context.DomainUsers.Add(domainUser);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.AddAsync(applicationUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(applicationUser.Id, result.Id);
            Assert.Equal(applicationUser.Email, result.Email);
            
            var savedUser = await context.Users.FindAsync(applicationUser.Id);
            Assert.NotNull(savedUser);
            Assert.Equal(applicationUser.Email, savedUser.Email);
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateAsync_ShouldUpdateApplicationUser()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ApplicationUserRepository(context);
            
            var domainUser = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email("original@example.com"),
                DisplayName = "Original User"
            };

            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "original@example.com",
                Email = "original@example.com",
                DomainUserId = domainUser.Id,
                DomainUser = domainUser
            };

            context.DomainUsers.Add(domainUser);
            context.Users.Add(applicationUser);
            await context.SaveChangesAsync();

            // Act
            applicationUser.Email = "updated@example.com";
            applicationUser.UserName = "updated@example.com";
            var result = await repository.UpdateAsync(applicationUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("updated@example.com", result.Email);
            Assert.Equal("updated@example.com", result.UserName);
            
            var updatedUser = await context.Users.FindAsync(applicationUser.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal("updated@example.com", updatedUser.Email);
            Assert.Equal("updated@example.com", updatedUser.UserName);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteAsync_ShouldRemoveApplicationUser_WhenUserExists()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ApplicationUserRepository(context);
            
            var domainUser = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email("test@example.com"),
                DisplayName = "Test User"
            };

            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "test@example.com",
                Email = "test@example.com",
                DomainUserId = domainUser.Id,
                DomainUser = domainUser
            };

            context.DomainUsers.Add(domainUser);
            context.Users.Add(applicationUser);
            await context.SaveChangesAsync();

            // Act
            await repository.DeleteAsync(applicationUser.Id);

            // Assert
            var deletedUser = await context.Users.FindAsync(applicationUser.Id);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteAsync_ShouldNotThrow_WhenUserDoesNotExist()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ApplicationUserRepository(context);
            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => repository.DeleteAsync(nonExistentId));
            Assert.Null(exception);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByIdAsync_ShouldWorkWithMultipleUsers()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ApplicationUserRepository(context);
            
            var users = new List<(User domainUser, ApplicationUser appUser)>();
            
            for (int i = 1; i <= 5; i++)
            {
                var domainUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = new Email($"user{i}@example.com"),
                    DisplayName = $"User {i}"
                };

                var applicationUser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = $"user{i}@example.com",
                    Email = $"user{i}@example.com",
                    DomainUserId = domainUser.Id,
                    DomainUser = domainUser
                };

                users.Add((domainUser, applicationUser));
                context.DomainUsers.Add(domainUser);
                context.Users.Add(applicationUser);
            }
            
            await context.SaveChangesAsync();

            // Act & Assert
            foreach (var (domainUser, appUser) in users)
            {
                var result = await repository.GetByIdAsync(appUser.Id);
                Assert.NotNull(result);
                Assert.Equal(appUser.Id, result.Id);
                Assert.Equal(appUser.Email, result.Email);
            }
        }

        [Fact]
        public async System.Threading.Tasks.Task GetAllAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ApplicationUserRepository(context);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }

    // Test-specific DbContext without seed data
    public class TestApplicationDbContext : ApplicationDbContext
    {
        public TestApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IDataProtectionProvider dataProtectionProvider) : base(options, dataProtectionProvider)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Don't call SeedData(modelBuilder) for tests
        }
    }
}