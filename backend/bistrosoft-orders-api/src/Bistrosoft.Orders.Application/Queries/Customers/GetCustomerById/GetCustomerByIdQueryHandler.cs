using Bistrosoft.Orders.Application.DTOs;
using Bistrosoft.Orders.Application.Interfaces;
using Bistrosoft.Orders.Domain.Exceptions;
using MediatR;

namespace Bistrosoft.Orders.Application.Queries.Customers.GetCustomerById;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDto> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdWithOrdersAsync(request.CustomerId, cancellationToken);

        if (customer is null)
        {
            throw new NotFoundException($"Customer with ID '{request.CustomerId}' not found.");
        }

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email.Value,
            PhoneNumber = customer.PhoneNumber,
            Orders = customer.Orders.Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt,
                Status = o.Status != null ? new OrderStatusDto
                {
                    Id = o.Status.Id,
                    Name = o.Status.Name,
                    Description = o.Status.Description
                } : new OrderStatusDto()
            }).ToList()
        };
    }
}
