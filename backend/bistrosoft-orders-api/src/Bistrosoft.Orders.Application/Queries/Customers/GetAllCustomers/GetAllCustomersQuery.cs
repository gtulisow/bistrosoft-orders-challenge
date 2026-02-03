using Bistrosoft.Orders.Application.DTOs;
using MediatR;

namespace Bistrosoft.Orders.Application.Queries.Customers.GetAllCustomers;

public class GetAllCustomersQuery : IRequest<IReadOnlyList<CustomerListDto>>
{
}
