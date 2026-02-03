using Bistrosoft.Orders.Application.Interfaces;
using Bistrosoft.Orders.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Bistrosoft.Orders.Infrastructure.Security;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<User> _passwordHasher;

    public PasswordService()
    {
        _passwordHasher = new PasswordHasher<User>();
    }

    public string HashPassword(string password)
    {
        // Use null as the user instance since we only need the hashing functionality
        return _passwordHasher.HashPassword(null!, password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(null!, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success || 
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
