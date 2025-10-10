using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Persistence.Repositories;
using Xunit;

namespace TodoApp.Infrastructure.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public UserRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private ApplicationDbContext CreateContext()
        {
            var context = new ApplicationDbContext(_options, seedData: false);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async System.Threading.Tasks.Task AddAsync_ShouldAddUserToDatabase()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var user = new User
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test User"
            };

            // Act
            var result = await repository.AddAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
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
                DisplayName = "Test User"
            };
            await repository.AddAsync(user);

            // Act
            var result = await repository.GetByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
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
        public async System.Threading.Tasks.Task GetAllAsync_ShouldReturnAllUsers()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var user1 = new User
            {
                Id = Guid.NewGuid(),
                DisplayName = "User 1"
            };
            var user2 = new User
            {
                Id = Guid.NewGuid(),
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
                DisplayName = "John Doe"
            };
            var user2 = new User
            {
                Id = Guid.NewGuid(),
                DisplayName = "Jane Smith"
            };
            var user3 = new User
            {
                Id = Guid.NewGuid(),
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
                DisplayName = "Original Name"
            };
            await repository.AddAsync(user);

            // Update user properties
            user.DisplayName = "Updated Name";

            // Act
            var result = await repository.UpdateAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.DisplayName);

            // Verify in database
            var userInDb = await repository.GetByIdAsync(user.Id);
            Assert.Equal("Updated Name", userInDb!.DisplayName);
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateAsync_ShouldUpdateUserGroupIds()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var user = new User
            {
                Id = Guid.NewGuid(),
                DisplayName = "Test User"
            };
            await repository.AddAsync(user);

            // Update user group memberships
            var group1 = new Group() { 
                Id = Guid.NewGuid(),
                Name = "Group 1"
            };
            var group2 = new Group()
            {
                Id = Guid.NewGuid(),
                Name = "Group 2"
            };
            var groupRepo = new GroupRepository(context);
            await groupRepo.AddAsync(group1);
            await groupRepo.AddAsync(group2);

            user.Groups.Add(group1);
            user.Groups.Add(group2);

            // Act
            var result = await repository.UpdateAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Groups.Count);
            Assert.Contains(group1, result.Groups);
            Assert.Contains(group2, result.Groups);

            // Verify in database
            var userInDb = await repository.GetByIdAsync(user.Id);
            Assert.Equal(2, userInDb!.Groups.Count);
            Assert.Contains(group1, result.Groups);
            Assert.Contains(group2, result.Groups);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteAsync_ShouldRemoveUser_WhenUserExists()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var user = new User
            {
                Id = Guid.NewGuid(),
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
        public async System.Threading.Tasks.Task SearchByNameAsync_ShouldReturnEmptyList_WhenNoMatches()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var user = new User
            {
                Id = Guid.NewGuid(),
                DisplayName = "John Doe"
            };
            await repository.AddAsync(user);

            // Act
            var result = await repository.SearchByNameAsync("NonExistent");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task SearchByNameAsync_ShouldBeCaseInsensitive()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UserRepository(context);
            var user = new User
            {
                Id = Guid.NewGuid(),
                DisplayName = "John Doe"
            };
            await repository.AddAsync(user);

            // Act
            var result = await repository.SearchByNameAsync("john");

            // Assert
            Assert.Single(result);
            Assert.Contains(result, u => u.Id == user.Id);
        }
    }
}