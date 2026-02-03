using Bistrosoft.Orders.Domain.Exceptions;
using Bistrosoft.Orders.Domain.ValueObjects;

namespace Bistrosoft.Orders.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }

    private User()
    {
    }

    public User(Email email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ValidationException("Password hash is required.");
        }

        Id = Guid.NewGuid();
        Email = email ?? throw new ValidationException("Email is required.");
        PasswordHash = passwordHash;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
