using System;
using System.Collections.Generic;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;
using Xunit;
using TaskStatus = TodoApp.Domain.Enums.TaskStatus;

namespace TodoApp.Domain.Tests.Entities
{
    public class TaskTests
    {
        [Fact]
        public void CreateTask_WithValidData_ShouldSetProperties()
        {
            // Arrange
            var description = "Test Task";
            var dueDate = DateTime.Now.AddDays(7);
            var status = TaskStatus.Pending;
            var userId = Guid.NewGuid();
            var groupId = Guid.NewGuid();
            var subTasks = new List<SubTask>
            {
                new SubTask { Id = Guid.NewGuid(), Description = "Sub Task 1", Status = TaskStatus.Pending }
            };

            // Act
            var task = new Domain.Entities.Task
            {
                Id = Guid.NewGuid(),
                Description = description,
                DueDate = dueDate,
                Status = status,
                UserId = userId,
                GroupId = groupId,
                SubTasks = subTasks
            };

            // Assert
            Assert.Equal(description, task.Description);
            Assert.Equal(dueDate, task.DueDate);
            Assert.Equal(status, task.Status);
            Assert.Equal(userId, task.UserId);
            Assert.Equal(groupId, task.GroupId);
            Assert.Equal(subTasks, task.SubTasks);
        }
    }
}