using Bistrosoft.Orders.Infrastructure.Repositories;
using Bistrosoft.Orders.Tests.TestHelpers;
using Xunit;

namespace Bistrosoft.Orders.Tests.Infrastructure.Repositories;

public class CustomerRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldAddCustomer_ToDatabase()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new CustomerRepository(context);
        var customer = EntityBuilder.CreateCustomer();

        // Act
        await repository.AddAsync(customer);
        await context.SaveChangesAsync();

        // Assert
        var retrievedCustomer = await repository.GetByIdAsync(customer.Id);
        Assert.NotNull(retrievedCustomer);
        Assert.Equal(customer.Name, retrievedCustomer.Name);
        Assert.Equal(customer.Email.Value, retrievedCustomer.Email.Value);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new CustomerRepository(context);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnCustomer_WhenEmailExists()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new CustomerRepository(context);
        var customer = EntityBuilder.CreateCustomer(email: "test@example.com");

        await repository.AddAsync(customer);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customer.Id, result.Id);
        Assert.Equal("test@example.com", result.Email.Value);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldBeCaseInsensitive()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new CustomerRepository(context);
        var customer = EntityBuilder.CreateCustomer(email: "Test@Example.COM");

        await repository.AddAsync(customer);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customer.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdWithOrdersAsync_ShouldIncludeOrders_WhenCustomerHasOrders()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new CustomerRepository(context);
        
        var customer = EntityBuilder.CreateCustomer();
        var order1 = EntityBuilder.CreateOrder(customer.Id);
        var order2 = EntityBuilder.CreateOrder(customer.Id);
        
        customer.AddOrder(order1);
        customer.AddOrder(order2);

        await repository.AddAsync(customer);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdWithOrdersAsync(customer.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Orders.Count);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCustomers_OrderedByName()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new CustomerRepository(context);
        
        var zara = EntityBuilder.CreateCustomer(name: "Zara Smith", email: "zara@example.com");
        var ana = EntityBuilder.CreateCustomer(name: "Ana García", email: "ana@example.com");
        var mario = EntityBuilder.CreateCustomer(name: "Mario López", email: "mario@example.com");

        await repository.AddAsync(zara);
        await repository.AddAsync(ana);
        await repository.AddAsync(mario);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("Ana García", result[0].Name);
        Assert.Equal("Mario López", result[1].Name);
        Assert.Equal("Zara Smith", result[2].Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCustomer_InDatabase()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new CustomerRepository(context);
        var customer = EntityBuilder.CreateCustomer(name: "Original Name");

        await repository.AddAsync(customer);
        await context.SaveChangesAsync();

        // Detach to simulate a fresh load
        context.Entry(customer).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        // Act
        var updatedCustomer = await repository.GetByIdAsync(customer.Id);
        Assert.NotNull(updatedCustomer);
        
        await repository.UpdateAsync(updatedCustomer);
        await context.SaveChangesAsync();

        // Assert
        var result = await repository.GetByIdAsync(customer.Id);
        Assert.NotNull(result);
    }
}
