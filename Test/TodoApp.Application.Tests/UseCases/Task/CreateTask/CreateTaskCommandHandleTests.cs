using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.Abstraction.Services;
using TodoApp.Application.UseCases.Task.CreateTask;
using Xunit;

namespace TodoApp.Application.Tests.UseCases.Task.CreateTask
{
    public class CreateTaskCommandHandleTests
    {
        [Fact]
        public async System.Threading.Tasks.Task Handle_WithValidCommand_CallsServiceAndReturnsTaskDto()
        {
            // Arrange
            var dto = new CreateTaskDto { Description = "Test", UserId = Guid.NewGuid() };
            var command = new CreateTaskCommand(dto);
            var expected = new TaskDto { Id = Guid.NewGuid(), Description = dto.Description, UserId = dto.UserId };
            var mockService = new Mock<ITaskService>();
            mockService.Setup(s => s.CreateTaskAsync(dto)).ReturnsAsync(expected);
            var handler = new CreateTaskCommandHandle(mockService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(expected, result);
            mockService.Verify(s => s.CreateTaskAsync(dto), Times.Once);
        }
    }
} 