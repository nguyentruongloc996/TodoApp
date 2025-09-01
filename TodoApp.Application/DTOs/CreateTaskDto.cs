using System;
using System.ComponentModel.DataAnnotations;
using TodoApp.Domain.ValueObjects;

namespace TodoApp.Application.DTOs
{
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
} 