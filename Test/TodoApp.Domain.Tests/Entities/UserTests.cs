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
            var email = new Email("user@example.com");
            var name = "Test User";
            var groupIds = new List<Guid> { Guid.NewGuid() };

            // Act
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Name = name,
                GroupIds = groupIds
            };

            // Assert
            Assert.Equal(email, user.Email);
            Assert.Equal(name, user.Name);
            Assert.Equal(groupIds, user.GroupIds);
        }
    }
} 