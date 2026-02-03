using Bistrosoft.Orders.Domain.Exceptions;

namespace Bistrosoft.Orders.Domain.Entities;

public class Order
{
    private readonly List<OrderItem> _orderItems = new();

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid StatusId { get; private set; }
    
    // Navigation property
    public OrderStatus? Status { get; private set; }
    
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    private Order()
    {
    }

    public Order(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            throw new ValidationException("CustomerId is required.");
        }

        Id = Guid.NewGuid();
        CustomerId = customerId;
        CreatedAt = DateTime.UtcNow;
        StatusId = OrderStatus.WellKnownStatuses.Pending;
        TotalAmount = 0m;
    }

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (productId == Guid.Empty)
        {
            throw new ValidationException("ProductId is required.");
        }

        var item = new OrderItem(Id, productId, quantity, unitPrice);
        _orderItems.Add(item);
        RecalculateTotal();
    }

    public void RecalculateTotal()
    {
        TotalAmount = _orderItems.Sum(item => item.Quantity * item.UnitPrice);
    }

    public void ChangeStatus(Guid newStatusId)
    {
        if (newStatusId == StatusId)
        {
            return;
        }

        if (!IsValidTransition(StatusId, newStatusId))
        {
            throw new InvalidOrderStatusTransitionException(
                $"Cannot transition from status {StatusId} to {newStatusId}");
        }

        StatusId = newStatusId;
    }

    private static bool IsValidTransition(Guid currentStatusId, Guid newStatusId)
    {
        // Pending -> Paid or Cancelled
        if (currentStatusId == OrderStatus.WellKnownStatuses.Pending)
        {
            return newStatusId == OrderStatus.WellKnownStatuses.Paid ||
                   newStatusId == OrderStatus.WellKnownStatuses.Cancelled;
        }

        // Paid -> Shipped
        if (currentStatusId == OrderStatus.WellKnownStatuses.Paid)
        {
            return newStatusId == OrderStatus.WellKnownStatuses.Shipped;
        }

        // Shipped -> Delivered
        if (currentStatusId == OrderStatus.WellKnownStatuses.Shipped)
        {
            return newStatusId == OrderStatus.WellKnownStatuses.Delivered;
        }

        return false;
    }
}
