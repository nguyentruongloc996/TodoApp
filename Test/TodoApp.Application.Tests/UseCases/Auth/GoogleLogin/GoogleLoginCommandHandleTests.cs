using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.Abstraction.Services;
using TodoApp.Application.UseCases.Auth.GoogleLogin;
using Xunit;

namespace TodoApp.Application.Tests.UseCases.Auth.GoogleLogin
{
    public class GoogleLoginCommandHandleTests
    {
        [Fact]
        public async System.Threading.Tasks.Task Handle_WithValidRequest_CallsServiceAndReturnsLoginResponse()
        {
            // Arrange
            var request = new GoogleAuthRequestDto { IdToken = "google-id-token" };
            var command = new GoogleLoginCommand(request);
            var expected = new LoginResponseDto 
            { 
                Token = "jwt-token", 
                RefreshToken = "refresh-token", 
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new ApplicationUserDto { Id = Guid.NewGuid(), Email = "test@example.com", Name = "Test User" }
            };
            var mockService = new Mock<IAuthService>();
            mockService.Setup(s => s.GoogleLoginAsync(request)).ReturnsAsync(expected);
            var handler = new GoogleLoginCommandHandle(mockService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(expected, result);
            mockService.Verify(s => s.GoogleLoginAsync(request), Times.Once);
        }
    }
} 