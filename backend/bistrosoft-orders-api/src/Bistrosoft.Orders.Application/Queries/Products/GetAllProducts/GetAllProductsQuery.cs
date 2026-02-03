using Bistrosoft.Orders.Application.DTOs;
using MediatR;

namespace Bistrosoft.Orders.Application.Queries.Products.GetAllProducts;

public class GetAllProductsQuery : IRequest<IReadOnlyList<ProductDto>>
{
}
