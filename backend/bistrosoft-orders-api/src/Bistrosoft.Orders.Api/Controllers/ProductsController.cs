using Bistrosoft.Orders.Application.DTOs;
using Bistrosoft.Orders.Application.Queries.Products.GetAllProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bistrosoft.Orders.Api.Controllers;

/// <summary>
/// Manages product operations
/// </summary>
[ApiController]
[Route("api/products")]
[Produces("application/json")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns the list of available products
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of products with their current stock</returns>
    /// <response code="200">Products retrieved successfully</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllProducts(CancellationToken cancellationToken)
    {
        var query = new GetAllProductsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
