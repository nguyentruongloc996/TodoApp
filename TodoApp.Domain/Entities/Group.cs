namespace TodoApp.Domain.Entities;

public class Group
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public List<Guid> MemberIds { get; set; } = new();
    public List<Guid> TaskIds { get; set; } = new();
} 