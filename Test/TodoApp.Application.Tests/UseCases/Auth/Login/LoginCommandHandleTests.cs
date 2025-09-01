using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.Abstraction.Services;
using TodoApp.Application.UseCases.Auth.Login;
using Xunit;

namespace TodoApp.Application.Tests.UseCases.Auth.Login
{
    public class LoginCommandHandleTests
    {
        [Fact]
        public async System.Threading.Tasks.Task Handle_WithValidRequest_CallsServiceAndReturnsLoginResponse()
        {
            // Arrange
            var request = new LoginRequestDto { Email = "test@example.com", Password = "password" };
            var command = new LoginCommand(request);
            var expected = new LoginResponseDto 
            { 
                Token = "jwt-token", 
                RefreshToken = "refresh-token", 
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new UserDto { Id = Guid.NewGuid(), Email = request.Email, Name = "Test User" }
            };
            var mockService = new Mock<IAuthService>();
            mockService.Setup(s => s.LoginAsync(request)).ReturnsAsync(expected);
            var handler = new LoginCommandHandle(mockService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(expected, result);
            mockService.Verify(s => s.LoginAsync(request), Times.Once);
        }
    }
} 