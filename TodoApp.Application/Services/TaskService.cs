using TodoApp.Application.Abstraction.Services;
using TodoApp.Application.DTOs;
using TodoApp.Application.Abstraction;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;

namespace TodoApp.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async System.Threading.Tasks.Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto)
        {
            var task = new Domain.Entities.Task
            {
                Id = Guid.NewGuid(),
                Description = createTaskDto.Description,
                DueDate = createTaskDto.DueDate,
                Reminder = createTaskDto.Reminder,
                RepeatPattern = createTaskDto.RepeatPattern,
                Status = TodoApp.Domain.Enums.TaskStatus.Pending,
                UserId = createTaskDto.UserId,
                GroupId = createTaskDto.GroupId
            };

            var createdTask = await _unitOfWork.Tasks.AddAsync(task);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(createdTask);
        }

        public async System.Threading.Tasks.Task<TaskDto> UpdateTaskAsync(UpdateTaskDto updateTaskDto)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(updateTaskDto.Id);
            if (task == null)
                throw new ArgumentException("Task not found");

            task.Description = updateTaskDto.Description;
            task.DueDate = updateTaskDto.DueDate;
            task.Reminder = updateTaskDto.Reminder;
            task.RepeatPattern = updateTaskDto.RepeatPattern;
            task.Status = updateTaskDto.Status;

            var updatedTask = await _unitOfWork.Tasks.UpdateAsync(task);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(updatedTask);
        }

        public async System.Threading.Tasks.Task<TaskDto> CompleteTaskAsync(Guid taskId)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
            if (task == null)
                throw new ArgumentException("Task not found");

            task.Status = TodoApp.Domain.Enums.TaskStatus.Completed;

            var completedTask = await _unitOfWork.Tasks.UpdateAsync(task);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(completedTask);
        }

        public async System.Threading.Tasks.Task<bool> DeleteTaskAsync(Guid taskId)
        {
            var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
            if (task == null)
                return false;

            await _unitOfWork.Tasks.DeleteAsync(taskId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private static TaskDto MapToDto(Domain.Entities.Task task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Description = task.Description,
                DueDate = task.DueDate,
                Reminder = task.Reminder,
                RepeatPattern = task.RepeatPattern,
                Status = task.Status,
                UserId = task.UserId,
                GroupId = task.GroupId
            };
        }

        public async Task<List<TaskDto>> GetTasksByMemberIdAsync(Guid userId)
        {
            var tasks = await _unitOfWork.Tasks.GetByUserIdAsync(userId);
            return tasks.Select(MapToDto).ToList();
        }
    }
}