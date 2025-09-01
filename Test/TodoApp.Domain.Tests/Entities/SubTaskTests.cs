using System;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;
using Xunit;

namespace TodoApp.Domain.Tests.Entities
{
    public class SubTaskTests
    {
        [Fact]
        public void CreateSubTask_WithValidData_ShouldSetProperties()
        {
            // Arrange
            var description = "Subtask description";
            var status = TodoApp.Domain.Enums.TaskStatus.Pending;
            var parentTaskId = Guid.NewGuid();

            // Act
            var subTask = new SubTask
            {
                Id = Guid.NewGuid(),
                Description = description,
                Status = status,
                ParentTaskId = parentTaskId
            };

            // Assert
            Assert.Equal(description, subTask.Description);
            Assert.Equal(status, subTask.Status);
            Assert.Equal(parentTaskId, subTask.ParentTaskId);
        }
    }
} 