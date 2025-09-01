using System;
using TodoApp.Domain;
using TodoApp.Domain.Enums;
using TodoApp.Domain.ValueObjects;
using Xunit;

namespace TodoApp.Domain.Tests.Entities
{
    public class TaskTests
    {
        [Fact]
        public void CreateTask_WithValidData_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var description = "Test task";
            var dueDate = DateTime.UtcNow.AddDays(1);
            var reminder = new Reminder { Time = dueDate.AddHours(-1), Message = "Reminder!" };
            var repeat = new RepeatPattern { Type = RepeatType.Daily, Interval = 1 };
            var userId = Guid.NewGuid();

            // Act
            var task = new Domain.Entities.Task
            {
                Id = Guid.NewGuid(),
                Description = description,
                DueDate = dueDate,
                Reminder = reminder,
                RepeatPattern = repeat,
                Status = Enums.TaskStatus.Pending,
                UserId = userId
            };

            // Assert
            Assert.Equal(description, task.Description);
            Assert.Equal(dueDate, task.DueDate);
            Assert.Equal(reminder, task.Reminder);
            Assert.Equal(repeat, task.RepeatPattern);
            Assert.Equal(Enums.TaskStatus.Pending, task.Status);
            Assert.Equal(userId, task.UserId);
        }
    }
} 