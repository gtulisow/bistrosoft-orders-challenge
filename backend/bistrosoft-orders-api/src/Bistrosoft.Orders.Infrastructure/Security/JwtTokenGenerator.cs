using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bistrosoft.Orders.Application.Interfaces;
using Bistrosoft.Orders.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Bistrosoft.Orders.Infrastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _key;
    private readonly int _expiresMinutes;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
        _issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer not configured");
        _audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience not configured");
        _key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
        
        var expiresMinutesStr = configuration["Jwt:ExpiresMinutes"];
        _expiresMinutes = int.TryParse(expiresMinutesStr, out var minutes) ? minutes : 60;

        if (_key.Length < 32)
        {
            throw new InvalidOperationException("Jwt:Key must be at least 32 characters long for security.");
        }
    }

    public string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expiresMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTime GetTokenExpiration()
    {
        return DateTime.UtcNow.AddMinutes(_expiresMinutes);
    }
}
