using Bistrosoft.Orders.Application.Interfaces;
using Bistrosoft.Orders.Application.Queries.Customers.GetCustomerById;
using Bistrosoft.Orders.Domain.Exceptions;
using Bistrosoft.Orders.Tests.Application.Common;
using Bistrosoft.Orders.Tests.TestHelpers;
using Moq;
using Xunit;

namespace Bistrosoft.Orders.Tests.Application.Customers.GetCustomerById;

public class GetCustomerByIdQueryHandlerTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly GetCustomerByIdQueryHandler _handler;

    public GetCustomerByIdQueryHandlerTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _handler = new GetCustomerByIdQueryHandler(_customerRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCustomerDto_WhenCustomerExists()
    {
        // Arrange
        var customerId = TestFixtures.Customers.ValidCustomerId;
        var customer = EntityBuilder.CreateCustomer();
        
        _customerRepositoryMock
            .Setup(x => x.GetByIdWithOrdersAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        var query = new GetCustomerByIdQuery { CustomerId = customerId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customer.Id, result.Id);
        Assert.Equal(customer.Name, result.Name);
        Assert.Equal(customer.Email.Value, result.Email);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenCustomerDoesNotExist()
    {
        // Arrange
        var customerId = TestFixtures.Customers.ValidCustomerId;
        
        _customerRepositoryMock
            .Setup(x => x.GetByIdWithOrdersAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Customer?)null);

        var query = new GetCustomerByIdQuery { CustomerId = customerId };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => 
            _handler.Handle(query, CancellationToken.None));
    }
}
