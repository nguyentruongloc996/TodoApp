using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.Abstraction.Services;
using TodoApp.Application.UseCases.User.SearchUsers;
using Xunit;

namespace TodoApp.Application.Tests.UseCases.User.SearchUsers
{
    public class SearchUsersQueryHandleTests
    {
        [Fact]
        public async System.Threading.Tasks.Task Handle_WithValidSearchTerm_CallsServiceAndReturnsUserList()
        {
            // Arrange
            var searchTerm = "test";
            var query = new SearchUsersQuery(searchTerm);
            var expected = new List<UserDto> 
            { 
                new UserDto { Id = Guid.NewGuid(), Email = "test1@example.com", Name = "Test User 1" },
                new UserDto { Id = Guid.NewGuid(), Email = "test2@example.com", Name = "Test User 2" }
            };
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.SearchUsersAsync(searchTerm)).ReturnsAsync(expected);
            var handler = new SearchUsersQueryHandle(mockService.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(expected, result);
            mockService.Verify(s => s.SearchUsersAsync(searchTerm), Times.Once);
        }
    }
} 