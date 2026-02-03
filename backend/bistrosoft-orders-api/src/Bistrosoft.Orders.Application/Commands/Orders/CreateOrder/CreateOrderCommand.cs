using Bistrosoft.Orders.Application.DTOs;
using MediatR;

namespace Bistrosoft.Orders.Application.Commands.Orders.CreateOrder;

public class CreateOrderCommand : IRequest<Guid>
{
    public Guid CustomerId { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
