using System;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;
using Xunit;
using TaskStatus = TodoApp.Domain.Enums.TaskStatus;

namespace TodoApp.Domain.Tests.Entities
{
    public class SubTaskTests
    {
        [Fact]
        public void CreateSubTask_WithValidData_ShouldSetProperties()
        {
            // Arrange
            var description = "Test SubTask";
            var status = TaskStatus.Pending;
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