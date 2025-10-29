using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Abstraction.Messaging;
using TodoApp.Application.DTOs;
using TodoApp.Application.UseCases.Task.CreateTask;
using TodoApp.Application.UseCases.Task.UpdateTask;
using TodoApp.Application.UseCases.Task.CompleteTask;
using TodoApp.Application.UseCases.Task.DeleteTask;
using Microsoft.AspNetCore.Authorization;

namespace TodoApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ICommandHandle<CreateTaskCommand, TaskDto> _createTaskCommandHandle;
        private readonly ICommandHandle<UpdateTaskCommand, TaskDto> _updateTaskCommandHandle;
        private readonly ICommandHandle<CompleteTaskCommand<TaskDto>, TaskDto> _completeTaskCommandHandle;
        private readonly ICommandHandle<DeleteTaskCommand, bool> _deleteTaskCommandHandle;

        public TaskController(
            ICommandHandle<CreateTaskCommand, TaskDto> createTaskCommandHandle,
            ICommandHandle<UpdateTaskCommand, TaskDto> updateTaskCommandHandle,
            ICommandHandle<CompleteTaskCommand<TaskDto>, TaskDto> completeTaskCommandHandle,
            ICommandHandle<DeleteTaskCommand, bool> deleteTaskCommandHandle)
        {
            _createTaskCommandHandle = createTaskCommandHandle;
            _updateTaskCommandHandle = updateTaskCommandHandle;
            _completeTaskCommandHandle = completeTaskCommandHandle;
            _deleteTaskCommandHandle = deleteTaskCommandHandle;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto, CancellationToken cancellationToken)
        {
            var command = new CreateTaskCommand(dto);
            var result = await _createTaskCommandHandle.Handle(command, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateTaskDto dto, CancellationToken cancellationToken)
        {
            var command = new UpdateTaskCommand(dto);
            var result = await _updateTaskCommandHandle.Handle(command, cancellationToken);
            return Ok(result);
        }

        [HttpPost("complete/{taskId}")]
        public async Task<IActionResult> Complete(Guid taskId, CancellationToken cancellationToken)
        {
            var command = new CompleteTaskCommand<TaskDto>(taskId);
            var result = await _completeTaskCommandHandle.Handle(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> Delete(Guid taskId, CancellationToken cancellationToken)
        {
            var command = new DeleteTaskCommand(taskId);
            var result = await _deleteTaskCommandHandle.Handle(command, cancellationToken);
            return Ok(result);
        }
    }
} 