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
            var members = new List<User>
            {
                new User { Id = Guid.NewGuid(), DisplayName = "User 1" },
                new User { Id = Guid.NewGuid(), DisplayName = "User 2" }
            };
            var tasks = new List<Domain.Entities.Task>
            {
                new Domain.Entities.Task { Id = Guid.NewGuid(), Description = "Test Task", UserId = Guid.NewGuid() }
            };

            // Act
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = name,
                Members = members,
                Tasks = tasks
            };

            // Assert
            Assert.Equal(name, group.Name);
            Assert.Equal(members, group.Members);
            Assert.Equal(tasks, group.Tasks);
        }
    }
}