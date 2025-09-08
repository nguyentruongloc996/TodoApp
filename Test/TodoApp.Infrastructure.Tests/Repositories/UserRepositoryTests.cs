using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Domain.ValueObjects;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Persistence.Repositories;
using Xunit;

namespace TodoApp.Infrastructure.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly IDataProtectionProvider dataProtectionProvider = DataProtectionProvider.Create("TestApp");

        public UserRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private ApplicationDbContext CreateContext() => new(_options, dataProtectionProvider);

        [Fact]
        public async System.Threading.Tasks.Task AddAsync_ShouldAddUserToDatabase()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email("test@example.com"),
                DisplayName = "Test User"
            };

            // Act
            var result = await repository.AddAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Email.Value, result.Email.Value);
            Assert.Equal(user.DisplayName, result.DisplayName);

            // Verify it's in the database
            var userInDb = await context.DomainUsers.FindAsync(user.Id);
            Assert.NotNull(userInDb);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email("test@example.com"),
                DisplayName = "Test User"
            };
            await repository.AddAsync(user);

            // Act
            var result = await repository.GetByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Email.Value, result.Email.Value);
            Assert.Equal(user.DisplayName, result.DisplayName);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await repository.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByEmailAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var email = "test@example.com";
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email(email),
                DisplayName = "Test User"
            };
            await repository.AddAsync(user);

            // Act
            var result = await repository.GetByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Email.Value, result.Email.Value);
            Assert.Equal(user.DisplayName, result.DisplayName);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var nonExistentEmail = "nonexistent@example.com";

            // Act
            var result = await repository.GetByEmailAsync(nonExistentEmail);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetAllAsync_ShouldReturnAllUsers()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var user1 = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email("user1@example.com"),
                DisplayName = "User 1"
            };
            var user2 = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email("user2@example.com"),
                DisplayName = "User 2"
            };
            await repository.AddAsync(user1);
            await repository.AddAsync(user2);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, u => u.Id == user1.Id);
            Assert.Contains(result, u => u.Id == user2.Id);
        }

        [Fact]
        public async System.Threading.Tasks.Task SearchByNameAsync_ShouldReturnMatchingUsers()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var user1 = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email("john@example.com"),
                DisplayName = "John Doe"
            };
            var user2 = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email("jane@example.com"),
                DisplayName = "Jane Smith"
            };
            var user3 = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email("johnny@example.com"),
                DisplayName = "Johnny Johnson"
            };
            await repository.AddAsync(user1);
            await repository.AddAsync(user2);
            await repository.AddAsync(user3);

            // Act
            var result = await repository.SearchByNameAsync("John");

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, u => u.Id == user1.Id);
            Assert.Contains(result, u => u.Id == user3.Id);
            Assert.DoesNotContain(result, u => u.Id == user2.Id);
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateAsync_ShouldUpdateExistingUser()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email("original@example.com"),
                DisplayName = "Original Name"
            };
            await repository.AddAsync(user);

            // Update user properties
            user.Email = new Email("updated@example.com");
            user.DisplayName = "Updated Name";

            // Act
            var result = await repository.UpdateAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("updated@example.com", result.Email.Value);
            Assert.Equal("Updated Name", result.DisplayName);

            // Verify in database
            var userInDb = await repository.GetByIdAsync(user.Id);
            Assert.Equal("updated@example.com", userInDb!.Email.Value);
            Assert.Equal("Updated Name", userInDb.DisplayName);
        }

        [Fact]
        public async    System.Threading.Tasks.Task DeleteAsync_ShouldRemoveUser_WhenUserExists()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email("test@example.com"),
                DisplayName = "Test User"
            };
            await repository.AddAsync(user);

            // Act
            await repository.DeleteAsync(user.Id);

            // Assert
            var userInDb = await repository.GetByIdAsync(user.Id);
            Assert.Null(userInDb);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteAsync_ShouldNotThrow_WhenUserDoesNotExist()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => repository.DeleteAsync(nonExistentId));
            Assert.Null(exception);
        }

        [Fact]
        public async System.Threading.Tasks.Task ExistsAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email("test@example.com"),
                DisplayName = "Test User"
            };
            await repository.AddAsync(user);

            // Act
            var result = await repository.ExistsAsync(user.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task ExistsAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await repository.ExistsAsync(nonExistentId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task ExistsByEmailAsync_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var email = "test@example.com";
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = new Email(email),
                DisplayName = "Test User"
            };
            await repository.AddAsync(user);

            var userById = await repository.GetByIdAsync(user.Id);

            // Act
            var result = await repository.ExistsByEmailAsync(email);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task ExistsByEmailAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var nonExistentEmail = "nonexistent@example.com";

            // Act
            var result = await repository.ExistsByEmailAsync(nonExistentEmail);

            // Assert
            Assert.False(result);
        }
    }
}