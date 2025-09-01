using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.Abstraction.Services;
using TodoApp.Application.UseCases.Task.DeleteTask;
using Xunit;

namespace TodoApp.Application.Tests.UseCases.Task.DeleteTask
{
    public class DeleteTaskCommandHandleTests
    {
        [Fact]
        public async System.Threading.Tasks.Task Handle_WithValidCommand_CallsServiceAndReturnsTrue()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var command = new DeleteTaskCommand(taskId);
            var mockService = new Mock<ITaskService>();
            mockService.Setup(s => s.DeleteTaskAsync(taskId)).ReturnsAsync(true);
            var handler = new DeleteTaskCommandHandle(mockService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            mockService.Verify(s => s.DeleteTaskAsync(taskId), Times.Once);
        }
    }
} 