using System;
using System.Collections.Generic;
using TodoApp.Domain.Entities;
using Xunit;

namespace TodoApp.Domain.Tests.Entities
{
    public class GroupTests
    {
        [Fact]
        public void CreateGroup_WithValidData_ShouldSetProperties()
        {
            // Arrange
            var name = "Test Group";
            var memberIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var taskIds = new List<Guid> { Guid.NewGuid() };

            // Act
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = name,
                MemberIds = memberIds,
                TaskIds = taskIds
            };

            // Assert
            Assert.Equal(name, group.Name);
            Assert.Equal(memberIds, group.MemberIds);
            Assert.Equal(taskIds, group.TaskIds);
        }
    }
} 