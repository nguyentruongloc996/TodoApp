using System.ComponentModel.DataAnnotations;
using TodoApp.Domain.ValueObjects;

namespace TodoApp.Application.DTOs
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public Reminder? Reminder { get; set; }
        public RepeatPattern? RepeatPattern { get; set; }
        public TodoApp.Domain.Enums.TaskStatus Status { get; set; }
        public Guid? GroupId { get; set; }
        public Guid UserId { get; set; }
    }

    public class CreateTaskDto
    {
        [Required]
        [StringLength(200)]
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public Reminder? Reminder { get; set; }
        public RepeatPattern? RepeatPattern { get; set; }
        public Guid? GroupId { get; set; }
        [Required]
        public Guid UserId { get; set; }
    }

    public class UpdateTaskDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public Reminder? Reminder { get; set; }
        public RepeatPattern? RepeatPattern { get; set; }
        public TodoApp.Domain.Enums.TaskStatus Status { get; set; }
        public Guid? GroupId { get; set; }
        [Required]
        public Guid UserId { get; set; }
    }
} 