namespace Bistrosoft.Orders.Domain.Exceptions;

public class ValidationException : DomainException
{
    public ValidationException(string message) : base(message)
    {
    }
}
