using Bistrosoft.Orders.Application.DTOs;
using Bistrosoft.Orders.Application.Interfaces;
using MediatR;

namespace Bistrosoft.Orders.Application.Queries.Orders.GetOrdersByCustomer;

public class GetOrdersByCustomerQueryHandler : IRequestHandler<GetOrdersByCustomerQuery, List<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public GetOrdersByCustomerQueryHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<List<OrderDto>> Handle(GetOrdersByCustomerQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

        if (orders.Count == 0)
        {
            return new List<OrderDto>();
        }

        // Get all unique product IDs from all orders
        var productIds = orders
            .SelectMany(o => o.OrderItems.Select(oi => oi.ProductId))
            .Distinct()
            .ToList();

        // Load products in batch
        var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
        var productDict = products.ToDictionary(p => p.Id);

        // Map to DTOs
        var result = orders.Select(order => new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            Status = order.Status != null ? new OrderStatusDto
            {
                Id = order.Status.Id,
                Name = order.Status.Name,
                Description = order.Status.Description
            } : new OrderStatusDto(),
            Items = order.OrderItems.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = productDict.ContainsKey(item.ProductId) 
                    ? productDict[item.ProductId].Name 
                    : "Unknown",
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.Quantity * item.UnitPrice
            }).ToList()
        }).ToList();

        return result;
    }
}
