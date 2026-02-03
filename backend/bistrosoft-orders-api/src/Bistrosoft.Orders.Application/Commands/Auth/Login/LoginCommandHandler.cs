using Bistrosoft.Orders.Application.DTOs;
using Bistrosoft.Orders.Application.Interfaces;
using Bistrosoft.Orders.Domain.Exceptions;
using MediatR;

namespace Bistrosoft.Orders.Application.Commands.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ValidationException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ValidationException("Password is required.");
        }

        // Find user by email
        var user = await _userRepository.FindByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        // Verify password
        var isPasswordValid = _passwordService.VerifyPassword(user.PasswordHash, request.Password);

        if (!isPasswordValid)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        // Generate JWT token
        var token = _jwtTokenGenerator.GenerateToken(user);
        var expiresAtUtc = _jwtTokenGenerator.GetTokenExpiration();

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            UserId = user.Id,
            Email = user.Email.Value
        };
    }
}
