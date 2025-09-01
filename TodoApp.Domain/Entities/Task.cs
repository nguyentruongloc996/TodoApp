namespace TodoApp.Domain.Entities;

using TodoApp.Domain.ValueObjects;
using TodoApp.Domain.Enums;

public class Task
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public Reminder? Reminder { get; set; }
    public RepeatPattern? RepeatPattern { get; set; }
    public TaskStatus Status { get; set; }
    public List<SubTask> SubTasks { get; set; } = new();
    public Guid? GroupId { get; set; }
    public Guid UserId { get; set; }
} 