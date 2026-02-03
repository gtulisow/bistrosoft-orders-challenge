using Bistrosoft.Orders.Domain.Entities;
using Bistrosoft.Orders.Infrastructure.Repositories;
using Bistrosoft.Orders.Tests.TestHelpers;
using Xunit;

namespace Bistrosoft.Orders.Tests.Infrastructure.Repositories;

public class OrderRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldAddOrder_ToDatabase()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new OrderRepository(context);
        
        var customer = EntityBuilder.CreateCustomer();
        var order = EntityBuilder.CreateOrder(customer.Id);

        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        // Act
        await repository.AddAsync(order);
        await context.SaveChangesAsync();

        // Assert
        var retrievedOrder = await repository.GetByIdAsync(order.Id);
        Assert.NotNull(retrievedOrder);
        Assert.Equal(customer.Id, retrievedOrder.CustomerId);
        Assert.Equal(OrderStatus.WellKnownStatuses.Pending, retrievedOrder.StatusId);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new OrderRepository(context);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByCustomerIdAsync_ShouldReturnAllOrders_ForSpecificCustomer()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new OrderRepository(context);
        
        var customer1 = EntityBuilder.CreateCustomer(email: "customer1@example.com");
        var customer2 = EntityBuilder.CreateCustomer(email: "customer2@example.com");
        
        var order1 = EntityBuilder.CreateOrder(customer1.Id);
        var order2 = EntityBuilder.CreateOrder(customer1.Id);
        var order3 = EntityBuilder.CreateOrder(customer2.Id);

        context.Customers.AddRange(customer1, customer2);
        await context.SaveChangesAsync();

        await repository.AddAsync(order1);
        await repository.AddAsync(order2);
        await repository.AddAsync(order3);
        await context.SaveChangesAsync();

        // Act
        var customer1Orders = await repository.GetByCustomerIdAsync(customer1.Id);

        // Assert
        Assert.Equal(2, customer1Orders.Count);
        Assert.All(customer1Orders, o => Assert.Equal(customer1.Id, o.CustomerId));
    }

    [Fact]
    public async Task GetByCustomerIdAsync_ShouldReturnEmptyList_WhenCustomerHasNoOrders()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new OrderRepository(context);
        var customerId = Guid.NewGuid();

        // Act
        var result = await repository.GetByCustomerIdAsync(customerId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateOrderStatus_InDatabase()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new OrderRepository(context);
        
        var customer = EntityBuilder.CreateCustomer();
        var order = EntityBuilder.CreateOrder(customer.Id);

        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        await repository.AddAsync(order);
        await context.SaveChangesAsync();

        // Act
        order.ChangeStatus(OrderStatus.WellKnownStatuses.Paid);
        await repository.UpdateAsync(order);
        await context.SaveChangesAsync();

        // Assert
        var updatedOrder = await repository.GetByIdAsync(order.Id);
        Assert.NotNull(updatedOrder);
        Assert.Equal(OrderStatus.WellKnownStatuses.Paid, updatedOrder.StatusId);
    }

    [Fact]
    public async Task AddAsync_ShouldAddOrderWithItems_ToDatabase()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new OrderRepository(context);
        
        var customer = EntityBuilder.CreateCustomer();
        var product = EntityBuilder.CreateProduct();
        var order = EntityBuilder.CreateOrderWithItems(customer.Id, 
            (product.Id, 2, 50m));

        context.Customers.Add(customer);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Act
        await repository.AddAsync(order);
        await context.SaveChangesAsync();

        // Assert
        var retrievedOrder = await repository.GetByIdAsync(order.Id);
        Assert.NotNull(retrievedOrder);
        Assert.Single(retrievedOrder.OrderItems);
        Assert.Equal(100m, retrievedOrder.TotalAmount);
    }
}
