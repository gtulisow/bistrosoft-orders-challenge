using Bistrosoft.Orders.Application.DTOs;
using MediatR;

namespace Bistrosoft.Orders.Application.Commands.Auth.Login;

public class LoginCommand : IRequest<LoginResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
