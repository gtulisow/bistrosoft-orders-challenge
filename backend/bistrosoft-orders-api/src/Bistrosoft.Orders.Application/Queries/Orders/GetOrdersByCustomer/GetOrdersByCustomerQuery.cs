using Bistrosoft.Orders.Application.DTOs;
using MediatR;

namespace Bistrosoft.Orders.Application.Queries.Orders.GetOrdersByCustomer;

public class GetOrdersByCustomerQuery : IRequest<List<OrderDto>>
{
    public Guid CustomerId { get; set; }
}
