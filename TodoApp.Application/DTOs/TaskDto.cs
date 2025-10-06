using System;
using TodoApp.Domain.Enums;
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
} 