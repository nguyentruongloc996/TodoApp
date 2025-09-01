namespace TodoApp.Domain.Entities;
using TodoApp.Domain.ValueObjects;

public class User
{
    public Guid Id { get; set; }
    public required Email Email { get; set; }
    public required string Name { get; set; }
    public List<Guid> GroupIds { get; set; } = new();
} 