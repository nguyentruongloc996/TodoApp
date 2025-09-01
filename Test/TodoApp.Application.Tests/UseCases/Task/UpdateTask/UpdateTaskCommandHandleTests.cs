using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.Abstraction.Services;
using TodoApp.Application.UseCases.Task.UpdateTask;
using Xunit;
using TaskDto = TodoApp.Application.DTOs.TaskDto;

namespace TodoApp.Application.Tests.UseCases.Task.UpdateTask
{
    public class UpdateTaskCommandHandleTests
    {
        [Fact]
        public async System.Threading.Tasks.Task Handle_WithValidCommand_CallsServiceAndReturnsTaskDto()
        {
            // Arrange
            var dto = new UpdateTaskDto { Id = Guid.NewGuid(), Description = "Updated", UserId = Guid.NewGuid() };
            var command = new UpdateTaskCommand(dto);
            var expected = new TaskDto { Id = dto.Id, Description = dto.Description, UserId = dto.UserId };
            var mockService = new Mock<ITaskService>();
            mockService.Setup(s => s.UpdateTaskAsync(dto)).ReturnsAsync(expected);
            var handler = new UpdateTaskCommandHandle(mockService.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(expected, result);
            mockService.Verify(s => s.UpdateTaskAsync(dto), Times.Once);
        }
    }
} 