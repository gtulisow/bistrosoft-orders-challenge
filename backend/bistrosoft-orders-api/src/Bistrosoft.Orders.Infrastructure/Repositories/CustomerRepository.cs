using Bistrosoft.Orders.Application.Interfaces;
using Bistrosoft.Orders.Domain.Entities;
using Bistrosoft.Orders.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bistrosoft.Orders.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _context.Customers.AddAsync(customer, cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLower();
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Email.Value.ToLower() == normalizedEmail, cancellationToken);
    }

    public async Task<Customer?> GetByIdWithOrdersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Include(c => c.Orders)
                .ThenInclude(o => o.OrderItems)
            .Include(c => c.Orders)
                .ThenInclude(o => o.Status)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Customers.Update(customer);
        return Task.CompletedTask;
    }
}
