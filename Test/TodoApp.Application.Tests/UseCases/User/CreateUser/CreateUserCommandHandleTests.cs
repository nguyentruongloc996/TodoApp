using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.Abstraction.Services;
using TodoApp.Application.UseCases.User.CreateUser;
using Xunit;

namespace TodoApp.Application.Tests.UseCases.User.CreateUser
{
    public class CreateUserCommandHandleTests
    {
        [Fact]
        public async System.Threading.Tasks.Task Handle_WithValidUserDto_CallsServiceAndReturnsUserDto()
        {
            // Arrange
            var userDto = new UserDto { Email = "test@example.com", Name = "Test User" };
            var command = new CreateUserCommand(userDto);
            var expected = new UserDto { IdentityId = Guid.NewGuid(), Email = userDto.Email, Name = userDto.Name };
            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.CreateUserAsync(userDto)).ReturnsAsync(expected);
            var handler = new CreateUserCommandHandle(mockService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(expected, result);
            mockService.Verify(s => s.CreateUserAsync(userDto), Times.Once);
        }
    }
} 