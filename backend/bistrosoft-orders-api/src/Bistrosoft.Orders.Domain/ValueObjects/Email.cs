using Bistrosoft.Orders.Domain.Exceptions;

namespace Bistrosoft.Orders.Domain.ValueObjects;

public sealed class Email : IEquatable<Email>
{
    public string Value { get; private set; } = string.Empty;

    private Email()
    {
    }

    public Email(string value)
    {
        Value = NormalizeAndValidate(value);
    }

    public override string ToString() => Value;

    public bool Equals(Email? other)
    {
        if (other is null)
        {
            return false;
        }

        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => obj is Email other && Equals(other);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    private static string NormalizeAndValidate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ValidationException("Email is required.");
        }

        var trimmed = value.Trim();
        var atIndex = trimmed.IndexOf('@');
        var lastAtIndex = trimmed.LastIndexOf('@');

        if (atIndex <= 0 || atIndex != lastAtIndex || atIndex == trimmed.Length - 1)
        {
            throw new ValidationException("Email format is invalid.");
        }

        var dotIndex = trimmed.IndexOf('.', atIndex + 1);
        if (dotIndex <= atIndex + 1 || dotIndex == trimmed.Length - 1)
        {
            throw new ValidationException("Email format is invalid.");
        }

        return trimmed;
    }
}
