using Bistrosoft.Orders.Domain.Exceptions;
using Bistrosoft.Orders.Domain.ValueObjects;

namespace Bistrosoft.Orders.Domain.Entities;

public class Customer
{
    private readonly List<Order> _orders = new();

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string? PhoneNumber { get; private set; }
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    private Customer()
    {
    }

    public Customer(string name, Email email, string? phoneNumber = null)
    {
        Id = Guid.NewGuid();
        Name = ValidateName(name);
        Email = email ?? throw new ValidationException("Email is required.");
        PhoneNumber = NormalizePhone(phoneNumber);
    }

    public void AddOrder(Order order)
    {
        if (order is null)
        {
            throw new ValidationException("Order is required.");
        }

        _orders.Add(order);
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Customer name is required.");
        }

        return name.Trim();
    }

    private static string? NormalizePhone(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return null;
        }

        return phoneNumber.Trim();
    }
}
