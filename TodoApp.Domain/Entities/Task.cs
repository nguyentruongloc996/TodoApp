namespace TodoApp.Domain.Entities;

using TodoApp.Domain.ValueObjects;
using TodoApp.Domain.Enums;

public class Task
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public Reminder? Reminder { get; set; }
    public RepeatPattern? RepeatPattern { get; set; }
    public TaskStatus Status { get; set; }
    public ICollection<SubTask> SubTasks { get; set; } = new List<SubTask>();
    public Guid? GroupId { get; set; }
    public Group? Group { get; set; }
    public required Guid UserId { get; set; }
    public User? User { get; set; }
} 