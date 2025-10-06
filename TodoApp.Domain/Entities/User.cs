namespace TodoApp.Domain.Entities;
using TodoApp.Domain.ValueObjects;

public class User
{
    public Guid Id { get; set; }
    public required string DisplayName { get; set; }
    public ICollection<Group> Groups { get; set; } = new List<Group>();
} 