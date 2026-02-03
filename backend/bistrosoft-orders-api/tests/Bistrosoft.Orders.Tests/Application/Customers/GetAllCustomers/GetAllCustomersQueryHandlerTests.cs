using Bistrosoft.Orders.Application.Interfaces;
using Bistrosoft.Orders.Application.Queries.Customers.GetAllCustomers;
using Bistrosoft.Orders.Tests.TestHelpers;
using Moq;
using Xunit;

namespace Bistrosoft.Orders.Tests.Application.Customers.GetAllCustomers;

public class GetAllCustomersQueryHandlerTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly GetAllCustomersQueryHandler _handler;

    public GetAllCustomersQueryHandlerTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _handler = new GetAllCustomersQueryHandler(_customerRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllCustomers_WhenCustomersExist()
    {
        // Arrange
        var customer1 = EntityBuilder.CreateCustomer("John Doe", "john@example.com", "+1234567890");
        var customer2 = EntityBuilder.CreateCustomer("Jane Smith", "jane@example.com", "+0987654321");
        var customer3 = EntityBuilder.CreateCustomer("Bob Johnson", "bob@example.com");

        _customerRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.Customer> { customer1, customer2, customer3 });

        var query = new GetAllCustomersQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, c => c.Name == "John Doe" && c.Email == "john@example.com");
        Assert.Contains(result, c => c.Name == "Jane Smith" && c.Email == "jane@example.com");
        Assert.Contains(result, c => c.Name == "Bob Johnson" && c.Email == "bob@example.com");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoCustomersExist()
    {
        // Arrange
        _customerRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.Customer>());

        var query = new GetAllCustomersQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_ShouldPreserveOrderByName_WhenMultipleCustomersExist()
    {
        // Arrange
        var ana = EntityBuilder.CreateCustomer("Ana García", "ana@example.com");
        var mario = EntityBuilder.CreateCustomer("Mario López", "mario@example.com");
        var zara = EntityBuilder.CreateCustomer("Zara Smith", "zara@example.com");

        // Repository returns customers already ordered by Name (repository responsibility)
        _customerRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.Customer> { ana, mario, zara });

        var query = new GetAllCustomersQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("Ana García", result[0].Name);
        Assert.Equal("Mario López", result[1].Name);
        Assert.Equal("Zara Smith", result[2].Name);
    }
}
