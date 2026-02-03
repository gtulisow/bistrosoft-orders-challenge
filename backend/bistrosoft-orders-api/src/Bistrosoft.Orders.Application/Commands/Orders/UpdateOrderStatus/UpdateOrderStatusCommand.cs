using MediatR;

namespace Bistrosoft.Orders.Application.Commands.Orders.UpdateOrderStatus;

public class UpdateOrderStatusCommand : IRequest<Unit>
{
    public Guid OrderId { get; set; }
    public Guid NewStatusId { get; set; }
}
