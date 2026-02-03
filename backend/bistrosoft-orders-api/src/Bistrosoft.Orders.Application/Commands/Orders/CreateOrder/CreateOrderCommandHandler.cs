using Bistrosoft.Orders.Application.Interfaces;
using Bistrosoft.Orders.Domain.Entities;
using Bistrosoft.Orders.Domain.Exceptions;
using MediatR;

namespace Bistrosoft.Orders.Application.Commands.Orders.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        if (request.Items == null || request.Items.Count == 0)
        {
            throw new ValidationException("Order must have at least one item.");
        }

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
            {
                throw new ValidationException("Item quantity must be greater than zero.");
            }
        }

        // Validate customer exists
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
        {
            throw new NotFoundException($"Customer with ID '{request.CustomerId}' not found.");
        }

        // Load all products
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

        // Validate all products exist
        if (products.Count != productIds.Distinct().Count())
        {
            var missingIds = productIds.Except(products.Select(p => p.Id));
            throw new NotFoundException($"Products not found: {string.Join(", ", missingIds)}");
        }

        // Create order
        var order = new Order(request.CustomerId);

        // Process each item
        foreach (var itemRequest in request.Items)
        {
            var product = products.First(p => p.Id == itemRequest.ProductId);

            // Validate stock
            if (product.StockQuantity < itemRequest.Quantity)
            {
                throw new ValidationException(
                    $"Insufficient stock for product '{product.Name}'. " +
                    $"Available: {product.StockQuantity}, Requested: {itemRequest.Quantity}");
            }

            // Add item to order (using product's current price)
            order.AddItem(product.Id, itemRequest.Quantity, product.Price);

            // Decrease stock
            product.DecreaseStock(itemRequest.Quantity);
            await _productRepository.UpdateAsync(product, cancellationToken);
        }

        // Recalculate total (although AddItem should handle this)
        order.RecalculateTotal();

        // Persist order
        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
