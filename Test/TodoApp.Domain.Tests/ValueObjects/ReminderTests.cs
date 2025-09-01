using System;
using TodoApp.Domain.ValueObjects;
using Xunit;

namespace TodoApp.Domain.Tests.ValueObjects
{
    public class ReminderTests
    {
        [Fact]
        public void CreateReminder_WithValidData_ShouldSetProperties()
        {
            // Arrange
            var time = DateTime.UtcNow.AddHours(1);
            var message = "Don't forget!";

            // Act
            var reminder = new Reminder { Time = time, Message = message };

            // Assert
            Assert.Equal(time, reminder.Time);
            Assert.Equal(message, reminder.Message);
        }
    }
} 