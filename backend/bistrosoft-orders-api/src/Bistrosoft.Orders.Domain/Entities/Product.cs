using Bistrosoft.Orders.Domain.Exceptions;

namespace Bistrosoft.Orders.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }

    private Product()
    {
    }

    public Product(string name, decimal price, int stockQuantity)
    {
        Id = Guid.NewGuid();
        Name = ValidateName(name);
        Price = ValidatePrice(price);
        StockQuantity = ValidateStock(stockQuantity);
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ValidationException("Quantity must be greater than zero.");
        }

        if (StockQuantity - quantity < 0)
        {
            throw new ValidationException("Insufficient stock.");
        }

        StockQuantity -= quantity;
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Product name is required.");
        }

        return name.Trim();
    }

    private static decimal ValidatePrice(decimal price)
    {
        if (price < 0)
        {
            throw new ValidationException("Product price cannot be negative.");
        }

        return price;
    }

    private static int ValidateStock(int stockQuantity)
    {
        if (stockQuantity < 0)
        {
            throw new ValidationException("Stock quantity cannot be negative.");
        }

        return stockQuantity;
    }
}
