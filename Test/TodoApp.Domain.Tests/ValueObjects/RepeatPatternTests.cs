using System;
using TodoApp.Domain.Enums;
using TodoApp.Domain.ValueObjects;
using Xunit;

namespace TodoApp.Domain.Tests.ValueObjects
{
    public class RepeatPatternTests
    {
        [Fact]
        public void CreateRepeatPattern_WithValidData_ShouldSetProperties()
        {
            // Arrange
            var type = RepeatType.Weekly;
            var interval = 2;
            var endDate = DateTime.UtcNow.AddMonths(1);

            // Act
            var repeat = new RepeatPattern { Type = type, Interval = interval, EndDate = endDate };

            // Assert
            Assert.Equal(type, repeat.Type);
            Assert.Equal(interval, repeat.Interval);
            Assert.Equal(endDate, repeat.EndDate);
        }
    }
} 