namespace Bistrosoft.Orders.Domain.Exceptions;

public class InvalidOrderStatusTransitionException : DomainException
{
    public InvalidOrderStatusTransitionException(string message)
        : base(message)
    {
    }
}
