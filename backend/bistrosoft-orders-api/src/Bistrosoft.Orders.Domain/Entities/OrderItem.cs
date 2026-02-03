using Bistrosoft.Orders.Domain.Exceptions;

namespace Bistrosoft.Orders.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    private OrderItem()
    {
    }

    public OrderItem(Guid orderId, Guid productId, int quantity, decimal unitPrice)
    {
        if (orderId == Guid.Empty)
        {
            throw new ValidationException("OrderId is required.");
        }

        if (productId == Guid.Empty)
        {
            throw new ValidationException("ProductId is required.");
        }

        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = productId;
        Quantity = ValidateQuantity(quantity);
        UnitPrice = ValidateUnitPrice(unitPrice);
    }

    private static int ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ValidationException("Quantity must be greater than zero.");
        }

        return quantity;
    }

    private static decimal ValidateUnitPrice(decimal unitPrice)
    {
        if (unitPrice < 0)
        {
            throw new ValidationException("Unit price cannot be negative.");
        }

        return unitPrice;
    }
}
