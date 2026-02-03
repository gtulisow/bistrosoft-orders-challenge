using Bistrosoft.Orders.Application.DTOs;
using Bistrosoft.Orders.Application.Interfaces;
using MediatR;

namespace Bistrosoft.Orders.Application.Queries.Customers.GetAllCustomers;

public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, IReadOnlyList<CustomerListDto>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetAllCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IReadOnlyList<CustomerListDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);

        var customerDtos = customers.Select(c => new CustomerListDto
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email.Value,
            PhoneNumber = c.PhoneNumber
        }).ToList();

        return customerDtos;
    }
}
