using Bistrosoft.Orders.Infrastructure.Repositories;
using Bistrosoft.Orders.Tests.TestHelpers;
using Xunit;

namespace Bistrosoft.Orders.Tests.Infrastructure.Repositories;

public class ProductRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldAddProduct_ToDatabase()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new ProductRepository(context);
        var product = EntityBuilder.CreateProduct(name: "Test Product", price: 99.99m, stockQuantity: 10);

        // Act
        await repository.AddAsync(product);
        await context.SaveChangesAsync();

        // Assert
        var retrievedProduct = await repository.GetByIdAsync(product.Id);
        Assert.NotNull(retrievedProduct);
        Assert.Equal("Test Product", retrievedProduct.Name);
        Assert.Equal(99.99m, retrievedProduct.Price);
        Assert.Equal(10, retrievedProduct.StockQuantity);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new ProductRepository(context);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdsAsync_ShouldReturnMatchingProducts_WhenIdsExist()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new ProductRepository(context);
        
        var product1 = EntityBuilder.CreateProduct("Product 1", 10m, 5);
        var product2 = EntityBuilder.CreateProduct("Product 2", 20m, 10);
        var product3 = EntityBuilder.CreateProduct("Product 3", 30m, 15);

        await repository.AddAsync(product1);
        await repository.AddAsync(product2);
        await repository.AddAsync(product3);
        await context.SaveChangesAsync();

        var idsToFind = new List<Guid> { product1.Id, product3.Id };

        // Act
        var result = await repository.GetByIdsAsync(idsToFind);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Id == product1.Id);
        Assert.Contains(result, p => p.Id == product3.Id);
        Assert.DoesNotContain(result, p => p.Id == product2.Id);
    }

    [Fact]
    public async Task GetByIdsAsync_ShouldReturnEmptyList_WhenNoMatchingIds()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new ProductRepository(context);
        
        var nonExistentIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        // Act
        var result = await repository.GetByIdsAsync(nonExistentIds);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct_InDatabase()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new ProductRepository(context);
        var product = EntityBuilder.CreateProduct(stockQuantity: 100);

        await repository.AddAsync(product);
        await context.SaveChangesAsync();

        // Act
        product.DecreaseStock(30);
        await repository.UpdateAsync(product);
        await context.SaveChangesAsync();

        // Assert
        var updatedProduct = await repository.GetByIdAsync(product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equal(70, updatedProduct.StockQuantity);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistStockChanges_ForMultipleProducts()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateDbContext();
        var repository = new ProductRepository(context);
        
        var product1 = EntityBuilder.CreateProduct("Product 1", 50m, 100);
        var product2 = EntityBuilder.CreateProduct("Product 2", 75m, 50);

        await repository.AddAsync(product1);
        await repository.AddAsync(product2);
        await context.SaveChangesAsync();

        // Act
        product1.DecreaseStock(10);
        product2.DecreaseStock(5);
        
        await repository.UpdateAsync(product1);
        await repository.UpdateAsync(product2);
        await context.SaveChangesAsync();

        // Assert
        var updated1 = await repository.GetByIdAsync(product1.Id);
        var updated2 = await repository.GetByIdAsync(product2.Id);
        
        Assert.NotNull(updated1);
        Assert.NotNull(updated2);
        Assert.Equal(90, updated1.StockQuantity);
        Assert.Equal(45, updated2.StockQuantity);
    }
}
