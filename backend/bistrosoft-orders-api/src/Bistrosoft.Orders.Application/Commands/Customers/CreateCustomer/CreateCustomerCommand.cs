using MediatR;

namespace Bistrosoft.Orders.Application.Commands.Customers.CreateCustomer;

public class CreateCustomerCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}
