using Bistrosoft.Orders.Application.Commands.Orders.CreateOrder;
using Bistrosoft.Orders.Application.Interfaces;
using Bistrosoft.Orders.Domain.Exceptions;
using Bistrosoft.Orders.Tests.Application.Common;
using Bistrosoft.Orders.Tests.TestHelpers;
using Moq;
using Xunit;

namespace Bistrosoft.Orders.Tests.Application.Orders.CreateOrder;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        _handler = new CreateOrderCommandHandler(
            _customerRepositoryMock.Object,
            _productRepositoryMock.Object,
            _orderRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateOrderAndDecrementStock_WhenDataIsValid()
    {
        // Arrange
        var customerId = TestFixtures.Customers.ValidCustomerId;
        var quantity = 2;
        var product = EntityBuilder.CreateProduct(
            name: "Test Product",
            price: 50m,
            stockQuantity: 10);

        var command = new CreateOrderCommand
        {
            CustomerId = customerId,
            Items = new List<CreateOrderItemDto>
            {
                new() { ProductId = product.Id, Quantity = quantity }
            }
        };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(EntityBuilder.CreateCustomer());

        _productRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.Product> { product });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        
        _orderRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Domain.Entities.Order>(o => 
                o.CustomerId == customerId &&
                o.TotalAmount == 100m), // 2 * 50
                It.IsAny<CancellationToken>()), 
            Times.Once);

        _productRepositoryMock.Verify(
            x => x.UpdateAsync(It.Is<Domain.Entities.Product>(p => 
                p.StockQuantity == 8), // 10 - 2
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenStockIsInsufficient()
    {
        // Arrange
        var customerId = TestFixtures.Customers.ValidCustomerId;
        var product = EntityBuilder.CreateProduct(
            name: "Test Product",
            price: 50m,
            stockQuantity: 1);

        var command = new CreateOrderCommand
        {
            CustomerId = customerId,
            Items = new List<CreateOrderItemDto>
            {
                new() { ProductId = product.Id, Quantity = 5 } // More than available
            }
        };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(EntityBuilder.CreateCustomer());

        _productRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.Product> { product });

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        // Arrange
        var customerId = TestFixtures.Customers.ValidCustomerId;
        var nonExistentProductId = Guid.NewGuid();

        var command = new CreateOrderCommand
        {
            CustomerId = customerId,
            Items = new List<CreateOrderItemDto>
            {
                new() { ProductId = nonExistentProductId, Quantity = 1 }
            }
        };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(EntityBuilder.CreateCustomer());

        _productRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.Product>());

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldCalculateTotalAmountCorrectly_WhenMultipleItems()
    {
        // Arrange
        var customerId = TestFixtures.Customers.ValidCustomerId;
        var product1 = EntityBuilder.CreateProduct("Product 1", 50m, 10);
        var product2 = EntityBuilder.CreateProduct("Product 2", 30m, 10);

        var command = new CreateOrderCommand
        {
            CustomerId = customerId,
            Items = new List<CreateOrderItemDto>
            {
                new() { ProductId = product1.Id, Quantity = 2 }, // 100
                new() { ProductId = product2.Id, Quantity = 3 }  // 90
            }
        };

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(EntityBuilder.CreateCustomer());

        _productRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.Product> { product1, product2 });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _orderRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Domain.Entities.Order>(o => 
                o.TotalAmount == 190m), // 100 + 90
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
