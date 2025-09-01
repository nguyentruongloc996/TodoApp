using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Persistence.Repositories;
using Xunit;
using TaskEntity = TodoApp.Domain.Entities.Task;

namespace TodoApp.Infrastructure.Tests.Repositories
{
    public class TaskRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public TaskRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async System.Threading.Tasks.Task AddAsync_ShouldAddTaskToDatabase()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new TaskRepository(context);
            var taskEntity = new TodoApp.Domain.Entities.Task
            {
                Id = Guid.NewGuid(),
                Description = "Test Task",
                Status = TodoApp.Domain.Enums.TaskStatus.Pending,
                UserId = Guid.NewGuid()
            };

            // Act
            var result = await repository.AddAsync(taskEntity);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskEntity.Id, result.Id);
            Assert.Equal(taskEntity.Description, result.Description);
            Assert.Equal(taskEntity.Status, result.Status);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByIdAsync_ShouldReturnTask_WhenTaskExists()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new TaskRepository(context);
            var taskEntity = new TodoApp.Domain.Entities.Task
            {
                Id = Guid.NewGuid(),
                Description = "Test Task",
                Status = TodoApp.Domain.Enums.TaskStatus.Pending,
                UserId = Guid.NewGuid()
            };
            await repository.AddAsync(taskEntity);

            // Act
            var result = await repository.GetByIdAsync(taskEntity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskEntity.Id, result.Id);
            Assert.Equal(taskEntity.Description, result.Description);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new TaskRepository(context);
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await repository.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetByUserIdAsync_ShouldReturnUserTasks()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new TaskRepository(context);
            var userId = Guid.NewGuid();
            var task1 = new TodoApp.Domain.Entities.Task
            {
                Id = Guid.NewGuid(),
                Description = "Task 1",
                Status = TodoApp.Domain.Enums.TaskStatus.Pending,
                UserId = userId
            };
            var task2 = new TodoApp.Domain.Entities.Task
            {
                Id = Guid.NewGuid(),
                Description = "Task 2",
                Status = TodoApp.Domain.Enums.TaskStatus.Completed,
                UserId = userId
            };
            await repository.AddAsync(task1);
            await repository.AddAsync(task2);

            // Act
            var result = await repository.GetByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, taskEntity => Assert.Equal(userId, taskEntity.UserId));
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateAsync_ShouldUpdateTask()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new TaskRepository(context);
            var taskEntity = new TodoApp.Domain.Entities.Task
            {
                Id = Guid.NewGuid(),
                Description = "Original Description",
                Status = TodoApp.Domain.Enums.TaskStatus.Pending,
                UserId = Guid.NewGuid()
            };
            await repository.AddAsync(taskEntity);

            // Act
            taskEntity.Description = "Updated Description";
            taskEntity.Status = TodoApp.Domain.Enums.TaskStatus.Completed;
            var result = await repository.UpdateAsync(taskEntity);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Description", result.Description);
            Assert.Equal(TodoApp.Domain.Enums.TaskStatus.Completed, result.Status);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteAsync_ShouldRemoveTask()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new TaskRepository(context);
            var taskEntity = new TodoApp.Domain.Entities.Task
            {
                Id = Guid.NewGuid(),
                Description = "Test Task",
                Status = TodoApp.Domain.Enums.TaskStatus.Pending,
                UserId = Guid.NewGuid()
            };
            await repository.AddAsync(taskEntity);

            // Act
            await repository.DeleteAsync(taskEntity.Id);

            // Assert
            var deletedTask = await repository.GetByIdAsync(taskEntity.Id);
            Assert.Null(deletedTask);
        }

        [Fact]
        public async System.Threading.Tasks.Task ExistsAsync_ShouldReturnTrue_WhenTaskExists()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new TaskRepository(context);
            var taskEntity = new TodoApp.Domain.Entities.Task
            {
                Id = Guid.NewGuid(),
                Description = "Test Task",
                Status = TodoApp.Domain.Enums.TaskStatus.Pending,
                UserId = Guid.NewGuid()
            };
            await repository.AddAsync(taskEntity);

            // Act
            var result = await repository.ExistsAsync(taskEntity.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task ExistsAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new TaskRepository(context);
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await repository.ExistsAsync(nonExistentId);

            // Assert
            Assert.False(result);
        }
    }
} 