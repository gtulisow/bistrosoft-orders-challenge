using Bistrosoft.Orders.Application.Commands.Customers.CreateCustomer;
using Bistrosoft.Orders.Application.DTOs;
using Bistrosoft.Orders.Application.Queries.Customers.GetAllCustomers;
using Bistrosoft.Orders.Application.Queries.Customers.GetCustomerById;
using Bistrosoft.Orders.Application.Queries.Orders.GetOrdersByCustomer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bistrosoft.Orders.Api.Controllers;

/// <summary>
/// Manages customer operations
/// </summary>
[ApiController]
[Route("api/customers")]
[Produces("application/json")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns all registered customers in the system
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of customers ordered by name</returns>
    /// <response code="200">Customers retrieved successfully</response>
    /// <response code="401">Unauthorized - JWT token required</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<CustomerListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllCustomers(CancellationToken cancellationToken)
    {
        var query = new GetAllCustomersQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new customer
    /// </summary>
    /// <param name="request">Customer data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created customer ID</returns>
    /// <response code="201">Customer created successfully</response>
    /// <response code="400">Invalid request or email already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCustomer(
        [FromBody] CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customerId = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetCustomerById), new { id = customerId }, customerId);
    }

    /// <summary>
    /// Gets a customer by ID including their orders
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Customer details with orders</returns>
    /// <response code="200">Customer found</response>
    /// <response code="404">Customer not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetCustomerByIdQuery { CustomerId = id };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets all orders for a specific customer including items and product details
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of orders with items and product information</returns>
    /// <response code="200">Orders retrieved successfully (may be empty list)</response>
    [HttpGet("{id:guid}/orders")]
    [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomerOrders(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetOrdersByCustomerQuery { CustomerId = id };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
