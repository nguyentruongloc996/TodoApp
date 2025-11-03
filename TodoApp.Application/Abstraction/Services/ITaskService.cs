using System;
using System.Threading.Tasks;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Abstraction.Services
{
    public interface ITaskService
    {
        Task<TaskDto> CreateTaskAsync(CreateTaskDto dto);
        Task<TaskDto> UpdateTaskAsync(UpdateTaskDto dto);
        Task<TaskDto> CompleteTaskAsync(Guid taskId);
        Task<bool> DeleteTaskAsync(Guid taskId);
        Task<List<TaskDto>> GetTasksByMemberIdAsync(Guid userId);
    }
} 