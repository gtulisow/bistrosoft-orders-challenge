using Bistrosoft.Orders.Application.Commands.Orders.CreateOrder;
using Bistrosoft.Orders.Application.Commands.Orders.UpdateOrderStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bistrosoft.Orders.Api.Controllers;

/// <summary>
/// Manages order operations
/// </summary>
[ApiController]
[Route("api/orders")]
[Produces("application/json")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new order with items, validates stock availability, and decreases product stock
    /// </summary>
    /// <param name="request">Order data with customer ID and items</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created order ID</returns>
    /// <response code="201">Order created successfully</response>
    /// <response code="400">Invalid request, insufficient stock, or validation error</response>
    /// <response code="404">Customer or product not found</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var orderId = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(CreateOrder), new { id = orderId }, orderId);
    }

    /// <summary>
    /// Updates the status of an existing order
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="request">New status (must follow valid transitions: Pending→Paid→Shipped→Delivered or Pending→Cancelled)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Status updated successfully</response>
    /// <response code="404">Order not found</response>
    /// <response code="409">Invalid status transition</response>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateOrderStatus(
        Guid id,
        [FromBody] UpdateOrderStatusCommand request,
        CancellationToken cancellationToken)
    {
        request.OrderId = id;
        await _mediator.Send(request, cancellationToken);
        return NoContent();
    }
}
