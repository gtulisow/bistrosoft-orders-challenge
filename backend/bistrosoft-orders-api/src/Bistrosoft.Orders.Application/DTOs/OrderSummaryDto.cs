namespace Bistrosoft.Orders.Application.DTOs;

public class OrderSummaryDto
{
    public Guid Id { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatusDto Status { get; set; } = new();
}
