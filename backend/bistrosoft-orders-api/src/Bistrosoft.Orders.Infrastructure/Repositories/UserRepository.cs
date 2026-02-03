using Bistrosoft.Orders.Application.Interfaces;
using Bistrosoft.Orders.Domain.Entities;
using Bistrosoft.Orders.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bistrosoft.Orders.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToLower();
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Value.ToLower() == normalizedEmail, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }
}
