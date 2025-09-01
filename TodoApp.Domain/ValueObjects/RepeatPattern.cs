namespace TodoApp.Domain.ValueObjects;

using TodoApp.Domain.Enums;

public class RepeatPattern
{
    public RepeatType Type { get; set; }
    public int Interval { get; set; } // e.g., every X days
    public DateTime? EndDate { get; set; }
} 