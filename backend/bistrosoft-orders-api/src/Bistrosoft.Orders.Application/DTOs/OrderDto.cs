namespace Bistrosoft.Orders.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatusDto Status { get; set; } = new();
    public List<OrderItemDto> Items { get; set; } = new();
}
