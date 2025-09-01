namespace TodoApp.Domain.Entities;

using TodoApp.Domain.Enums;

public class SubTask
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public TaskStatus Status { get; set; }
    public Guid ParentTaskId { get; set; }
} 