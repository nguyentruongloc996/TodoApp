using Microsoft.EntityFrameworkCore;
using Moq;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Services;
using Xunit;
using TaskEntity = TodoApp.Domain.Entities.Task;

namespace TodoApp.Infrastructure.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public TaskServiceTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateTaskAsync_ShouldCreateNewTask()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var unitOfWork = new UnitOfWork(context);
            var service = new TaskService(unitOfWork);
            var createTaskDto = new CreateTaskDto
            {
                Description = "Test Task",
                UserId = Guid.NewGuid(),
                GroupId = null
            };

            // Act
            var result = await service.CreateTaskAsync(createTaskDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createTaskDto.Description, result.Description);
            Assert.Equal(createTaskDto.UserId, result.UserId);
            Assert.Equal(TodoApp.Domain.Enums.TaskStatus.Pending, result.Status);
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateTaskAsync_ShouldUpdateExistingTask()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var unitOfWork = new UnitOfWork(context);
            var service = new TaskService(unitOfWork);
            
            // Create a task first
            var createTaskDto = new CreateTaskDto
            {
                Description = "Original Task",
                UserId = Guid.NewGuid(),
                GroupId = null
            };
            var createdTask = await service.CreateTaskAsync(createTaskDto);

            // Update the task
            var updateTaskDto = new UpdateTaskDto
            {
                Id = createdTask.Id,
                Description = "Updated Task",
                Status = TodoApp.Domain.Enums.TaskStatus.Completed,
                UserId = createdTask.UserId,
                GroupId = null
            };

            // Act
            var result = await service.UpdateTaskAsync(updateTaskDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Task", result.Description);
            Assert.Equal(TodoApp.Domain.Enums.TaskStatus.Completed, result.Status);
        }

        [Fact]
        public async System.Threading.Tasks.Task CompleteTaskAsync_ShouldMarkTaskAsCompleted()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var unitOfWork = new UnitOfWork(context);
            var service = new TaskService(unitOfWork);
            
            var createTaskDto = new CreateTaskDto
            {
                Description = "Test Task",
                UserId = Guid.NewGuid(),
                GroupId = null
            };
            var createdTask = await service.CreateTaskAsync(createTaskDto);

            // Act
            var result = await service.CompleteTaskAsync(createdTask.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TodoApp.Domain.Enums.TaskStatus.Completed, result.Status);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteTaskAsync_ShouldReturnTrue_WhenTaskExists()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var unitOfWork = new UnitOfWork(context);
            var service = new TaskService(unitOfWork);
            
            var createTaskDto = new CreateTaskDto
            {
                Description = "Test Task",
                UserId = Guid.NewGuid(),
                GroupId = null
            };
            var createdTask = await service.CreateTaskAsync(createTaskDto);

            // Act
            var result = await service.DeleteTaskAsync(createdTask.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteTaskAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var unitOfWork = new UnitOfWork(context);
            var service = new TaskService(unitOfWork);
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await service.DeleteTaskAsync(nonExistentId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateTaskAsync_ShouldThrowException_WhenTaskNotFound()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var unitOfWork = new UnitOfWork(context);
            var service = new TaskService(unitOfWork);
            var nonExistentId = Guid.NewGuid();
            
            var updateTaskDto = new UpdateTaskDto
            {
                Id = nonExistentId,
                Description = "Updated Task",
                Status = TodoApp.Domain.Enums.TaskStatus.Completed,
                UserId = Guid.NewGuid(),
                GroupId = null
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateTaskAsync(updateTaskDto));
        }

        [Fact]
        public async System.Threading.Tasks.Task CompleteTaskAsync_ShouldThrowException_WhenTaskNotFound()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var unitOfWork = new UnitOfWork(context);
            var service = new TaskService(unitOfWork);
            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CompleteTaskAsync(nonExistentId));
        }
    }
} 