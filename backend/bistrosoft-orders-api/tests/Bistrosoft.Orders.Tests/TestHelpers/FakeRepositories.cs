using Bistrosoft.Orders.Application.Interfaces;
using Bistrosoft.Orders.Domain.Entities;

namespace Bistrosoft.Orders.Tests.TestHelpers;

public class FakeCustomerRepository : ICustomerRepository
{
    private readonly Dictionary<Guid, Customer> _customers = new();

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _customers.TryGetValue(id, out var customer);
        return Task.FromResult(customer);
    }

    public Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var customer = _customers.Values.FirstOrDefault(c => 
            c.Email.Value.Equals(email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(customer);
    }

    public Task<Customer?> GetByIdWithOrdersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _customers.TryGetValue(id, out var customer);
        return Task.FromResult(customer);
    }

    public Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = _customers.Values.OrderBy(c => c.Name).ToList();
        return Task.FromResult<IReadOnlyList<Customer>>(customers);
    }

    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _customers[customer.Id] = customer;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _customers[customer.Id] = customer;
        return Task.CompletedTask;
    }
}

public class FakeProductRepository : IProductRepository
{
    private readonly Dictionary<Guid, Product> _products = new();

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _products.TryGetValue(id, out var product);
        return Task.FromResult(product);
    }

    public Task<List<Product>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        var products = _products.Values.Where(p => ids.Contains(p.Id)).ToList();
        return Task.FromResult(products);
    }

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = _products.Values.OrderBy(p => p.Name).ToList();
        return Task.FromResult<IReadOnlyList<Product>>(products);
    }

    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _products[product.Id] = product;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _products[product.Id] = product;
        return Task.CompletedTask;
    }

    public void Add(Product product)
    {
        _products[product.Id] = product;
    }
}

public class FakeOrderRepository : IOrderRepository
{
    private readonly Dictionary<Guid, Order> _orders = new();

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task<List<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var orders = _orders.Values.Where(o => o.CustomerId == customerId).ToList();
        return Task.FromResult(orders);
    }

    public Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        _orders[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        _orders[order.Id] = order;
        return Task.CompletedTask;
    }
}
