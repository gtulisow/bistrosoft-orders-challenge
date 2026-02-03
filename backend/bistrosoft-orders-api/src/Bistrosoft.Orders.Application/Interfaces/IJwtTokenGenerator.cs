using Bistrosoft.Orders.Domain.Entities;

namespace Bistrosoft.Orders.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
    DateTime GetTokenExpiration();
}
