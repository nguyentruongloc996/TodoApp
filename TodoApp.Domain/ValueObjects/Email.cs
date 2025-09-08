using System.Text.RegularExpressions;

namespace TodoApp.Domain.ValueObjects;

public class Email
{
    public string Value { get; }
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.");

        if (!IsValid(value))
            throw new ArgumentException("Invalid email format.");

        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Email other)
            return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);

        if (obj is string str)
            return Value.Equals(str, StringComparison.OrdinalIgnoreCase);

        return base.Equals(obj);
    }

    public override int GetHashCode() => Value.ToLowerInvariant().GetHashCode();

    private static bool IsValid(string email)
    {
        // Updated regex pattern to handle edge cases properly
        var emailPattern = @"^[a-zA-Z0-9]+([._+-][a-zA-Z0-9]+)*@[a-zA-Z0-9]+([.-][a-zA-Z0-9]+)*\.[a-zA-Z]+$";

        return Regex.IsMatch(email, emailPattern);
    }
}