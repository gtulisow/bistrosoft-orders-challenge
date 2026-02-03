using Bistrosoft.Orders.Application.Interfaces;
using Bistrosoft.Orders.Application.Queries.Orders.GetOrdersByCustomer;
using Bistrosoft.Orders.Tests.Application.Common;
using Bistrosoft.Orders.Tests.TestHelpers;
using Moq;
using Xunit;

namespace Bistrosoft.Orders.Tests.Application.Orders.GetOrdersByCustomer;

public class GetOrdersByCustomerQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly GetOrdersByCustomerQueryHandler _handler;

    public GetOrdersByCustomerQueryHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _handler = new GetOrdersByCustomerQueryHandler(
            _orderRepositoryMock.Object,
            _productRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOrdersList_WhenCustomerHasOrders()
    {
        // Arrange
        var customerId = TestFixtures.Customers.ValidCustomerId;
        var productId = TestFixtures.Products.ValidProductId;
        
        var product = EntityBuilder.CreateProduct(name: "Test Product", price: 50m, stockQuantity: 10);
        
        var order1 = EntityBuilder.CreateOrderWithItems(customerId, 
            (productId, 2, 50m));
        var order2 = EntityBuilder.CreateOrderWithItems(customerId, 
            (productId, 1, 30m));

        _orderRepositoryMock
            .Setup(x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.Order> { order1, order2 });

        _productRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.Product> { product });

        var query = new GetOrdersByCustomerQuery { CustomerId = customerId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, o => o.TotalAmount == 100m); // 2 * 50
        Assert.Contains(result, o => o.TotalAmount == 30m);  // 1 * 30
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenCustomerHasNoOrders()
    {
        // Arrange
        var customerId = TestFixtures.Customers.ValidCustomerId;
        
        _orderRepositoryMock
            .Setup(x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.Order>());

        var query = new GetOrdersByCustomerQuery { CustomerId = customerId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
