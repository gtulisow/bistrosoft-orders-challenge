namespace Bistrosoft.Orders.Domain.Exceptions;

/// <summary>
/// Exception thrown when an authenticated user attempts to access a resource they don't have permission for.
/// Maps to HTTP 403 Forbidden.
/// </summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException(string message) : base(message)
    {
    }
}
