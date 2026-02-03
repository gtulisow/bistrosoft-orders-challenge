using Bistrosoft.Orders.Domain.Entities;
using Bistrosoft.Orders.Domain.ValueObjects;

namespace Bistrosoft.Orders.Tests.TestHelpers;

public static class EntityBuilder
{
    public static Customer CreateCustomer(
        string name = "John Doe",
        string email = "john.doe@example.com",
        string? phoneNumber = null)
    {
        var emailVo = new Email(email);
        return new Customer(name, emailVo, phoneNumber);
    }

    public static Product CreateProduct(
        string name = "Sample Product",
        decimal price = 100m,
        int stockQuantity = 50)
    {
        return new Product(name, price, stockQuantity);
    }

    public static Order CreateOrder(Guid customerId)
    {
        return new Order(customerId);
    }

    public static Order CreateOrderWithItems(
        Guid customerId,
        params (Guid productId, int quantity, decimal unitPrice)[] items)
    {
        var order = new Order(customerId);
        
        foreach (var (productId, quantity, unitPrice) in items)
        {
            order.AddItem(productId, quantity, unitPrice);
        }

        return order;
    }

    public static OrderItem CreateOrderItem(
        Guid orderId,
        Guid productId,
        int quantity = 1,
        decimal unitPrice = 100m)
    {
        return new OrderItem(orderId, productId, quantity, unitPrice);
    }
}
