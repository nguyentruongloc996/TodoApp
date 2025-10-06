namespace TodoApp.Domain.Entities;

public class Group
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public ICollection<User> Members { get; set; } = new List<User>();
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
} 