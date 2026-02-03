using Bistrosoft.Orders.Application.Interfaces;
using Bistrosoft.Orders.Domain.Entities;
using Bistrosoft.Orders.Domain.Exceptions;
using Bistrosoft.Orders.Domain.ValueObjects;
using MediatR;

namespace Bistrosoft.Orders.Application.Commands.Customers.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        // Validate email uniqueness
        var existingCustomer = await _customerRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingCustomer is not null)
        {
            throw new ValidationException($"A customer with email '{request.Email}' already exists.");
        }

        // Create Email value object (validates format)
        var email = new Email(request.Email);

        // Create domain entity (validates name, etc.)
        var customer = new Customer(request.Name, email, request.PhoneNumber);

        // Persist
        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}
