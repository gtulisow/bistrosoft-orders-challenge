using Bistrosoft.Orders.Domain.Exceptions;

namespace Bistrosoft.Orders.Domain.Entities;

public class OrderStatus
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    // Navigation property
    public ICollection<Order> Orders { get; private set; } = new List<Order>();

    private OrderStatus()
    {
    }

    public OrderStatus(Guid id, string name, string description)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException("OrderStatus ID cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("OrderStatus name is required.");
        }

        Id = id;
        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
    }

    // Well-known status IDs (for consistency across the system)
    public static class WellKnownStatuses
    {
        public static readonly Guid Pending = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public static readonly Guid Paid = Guid.Parse("00000000-0000-0000-0000-000000000002");
        public static readonly Guid Shipped = Guid.Parse("00000000-0000-0000-0000-000000000003");
        public static readonly Guid Delivered = Guid.Parse("00000000-0000-0000-0000-000000000004");
        public static readonly Guid Cancelled = Guid.Parse("00000000-0000-0000-0000-000000000005");
    }
}
