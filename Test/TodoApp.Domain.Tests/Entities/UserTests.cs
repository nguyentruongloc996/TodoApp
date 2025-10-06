using System;
using System.Collections.Generic;
using TodoApp.Domain.Entities;
using TodoApp.Domain.ValueObjects;
using Xunit;

namespace TodoApp.Domain.Tests.Entities
{
    public class UserTests
    {
        [Fact]
        public void CreateUser_WithValidData_ShouldSetProperties()
        {
            // Arrange
            var name = "Test User";
            var groups = new List<Group>
            {
                new Group { Id = Guid.NewGuid(), Name = "Test Group 1" }
            };

            // Act
            var user = new User
            {
                Id = Guid.NewGuid(),
                DisplayName = name,
                Groups = groups
            };

            // Assert
            Assert.Equal(name, user.DisplayName);
            Assert.Equal(groups, user.Groups);
        }
    }
}