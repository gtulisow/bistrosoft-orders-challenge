using Bistrosoft.Orders.Application.Commands.Orders.UpdateOrderStatus;
using Bistrosoft.Orders.Application.Interfaces;
using Bistrosoft.Orders.Domain.Entities;
using Bistrosoft.Orders.Domain.Exceptions;
using Bistrosoft.Orders.Tests.Application.Common;
using Bistrosoft.Orders.Tests.TestHelpers;
using Moq;
using Xunit;

namespace Bistrosoft.Orders.Tests.Application.Orders.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateOrderStatusCommandHandler _handler;

    public UpdateOrderStatusCommandHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateOrderStatusCommandHandler(
            _orderRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    public static IEnumerable<object[]> ValidTransitions()
    {
        yield return new object[] { OrderStatus.WellKnownStatuses.Pending, OrderStatus.WellKnownStatuses.Paid };
        yield return new object[] { OrderStatus.WellKnownStatuses.Paid, OrderStatus.WellKnownStatuses.Shipped };
        yield return new object[] { OrderStatus.WellKnownStatuses.Shipped, OrderStatus.WellKnownStatuses.Delivered };
        yield return new object[] { OrderStatus.WellKnownStatuses.Pending, OrderStatus.WellKnownStatuses.Cancelled };
    }

    [Theory]
    [MemberData(nameof(ValidTransitions))]
    public async Task Handle_ShouldUpdateStatus_WhenTransitionIsValid(
        Guid currentStatusId, Guid newStatusId)
    {
        // Arrange
        var orderId = TestFixtures.Orders.ValidOrderId;
        var customerId = TestFixtures.Customers.ValidCustomerId;
        var order = EntityBuilder.CreateOrder(customerId);
        
        // Set initial status
        if (currentStatusId == OrderStatus.WellKnownStatuses.Paid)
        {
            order.ChangeStatus(OrderStatus.WellKnownStatuses.Paid);
        }
        else if (currentStatusId == OrderStatus.WellKnownStatuses.Shipped)
        {
            order.ChangeStatus(OrderStatus.WellKnownStatuses.Paid);
            order.ChangeStatus(OrderStatus.WellKnownStatuses.Shipped);
        }

        _orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var command = new UpdateOrderStatusCommand
        {
            OrderId = orderId,
            NewStatusId = newStatusId
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _orderRepositoryMock.Verify(
            x => x.UpdateAsync(It.Is<Domain.Entities.Order>(o => 
                o.StatusId == newStatusId), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    public static IEnumerable<object[]> InvalidTransitions()
    {
        yield return new object[] { OrderStatus.WellKnownStatuses.Pending, OrderStatus.WellKnownStatuses.Shipped };
        yield return new object[] { OrderStatus.WellKnownStatuses.Pending, OrderStatus.WellKnownStatuses.Delivered };
        yield return new object[] { OrderStatus.WellKnownStatuses.Paid, OrderStatus.WellKnownStatuses.Pending };
        yield return new object[] { OrderStatus.WellKnownStatuses.Paid, OrderStatus.WellKnownStatuses.Delivered };
        yield return new object[] { OrderStatus.WellKnownStatuses.Paid, OrderStatus.WellKnownStatuses.Cancelled };
        yield return new object[] { OrderStatus.WellKnownStatuses.Shipped, OrderStatus.WellKnownStatuses.Pending };
        yield return new object[] { OrderStatus.WellKnownStatuses.Shipped, OrderStatus.WellKnownStatuses.Paid };
        yield return new object[] { OrderStatus.WellKnownStatuses.Shipped, OrderStatus.WellKnownStatuses.Cancelled };
        yield return new object[] { OrderStatus.WellKnownStatuses.Delivered, OrderStatus.WellKnownStatuses.Pending };
        yield return new object[] { OrderStatus.WellKnownStatuses.Delivered, OrderStatus.WellKnownStatuses.Paid };
        yield return new object[] { OrderStatus.WellKnownStatuses.Delivered, OrderStatus.WellKnownStatuses.Shipped };
        yield return new object[] { OrderStatus.WellKnownStatuses.Delivered, OrderStatus.WellKnownStatuses.Cancelled };
        yield return new object[] { OrderStatus.WellKnownStatuses.Cancelled, OrderStatus.WellKnownStatuses.Pending };
        yield return new object[] { OrderStatus.WellKnownStatuses.Cancelled, OrderStatus.WellKnownStatuses.Paid };
    }

    [Theory]
    [MemberData(nameof(InvalidTransitions))]
    public async Task Handle_ShouldThrowInvalidOrderStatusTransitionException_WhenTransitionIsInvalid(
        Guid currentStatusId, Guid newStatusId)
    {
        // Arrange
        var orderId = TestFixtures.Orders.ValidOrderId;
        var customerId = TestFixtures.Customers.ValidCustomerId;
        var order = EntityBuilder.CreateOrder(customerId);
        
        // Set initial status
        if (currentStatusId == OrderStatus.WellKnownStatuses.Paid)
        {
            order.ChangeStatus(OrderStatus.WellKnownStatuses.Paid);
        }
        else if (currentStatusId == OrderStatus.WellKnownStatuses.Shipped)
        {
            order.ChangeStatus(OrderStatus.WellKnownStatuses.Paid);
            order.ChangeStatus(OrderStatus.WellKnownStatuses.Shipped);
        }
        else if (currentStatusId == OrderStatus.WellKnownStatuses.Delivered)
        {
            order.ChangeStatus(OrderStatus.WellKnownStatuses.Paid);
            order.ChangeStatus(OrderStatus.WellKnownStatuses.Shipped);
            order.ChangeStatus(OrderStatus.WellKnownStatuses.Delivered);
        }
        else if (currentStatusId == OrderStatus.WellKnownStatuses.Cancelled)
        {
            order.ChangeStatus(OrderStatus.WellKnownStatuses.Cancelled);
        }

        _orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var command = new UpdateOrderStatusCommand
        {
            OrderId = orderId,
            NewStatusId = newStatusId
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOrderStatusTransitionException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = TestFixtures.Orders.ValidOrderId;
        
        _orderRepositoryMock
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Order?)null);

        var command = new UpdateOrderStatusCommand
        {
            OrderId = orderId,
            NewStatusId = OrderStatus.WellKnownStatuses.Paid
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
}
