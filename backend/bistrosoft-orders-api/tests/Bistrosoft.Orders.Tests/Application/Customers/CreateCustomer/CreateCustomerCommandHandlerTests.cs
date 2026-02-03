using Bistrosoft.Orders.Application.Commands.Customers.CreateCustomer;
using Bistrosoft.Orders.Application.Interfaces;
using Bistrosoft.Orders.Domain.Exceptions;
using Bistrosoft.Orders.Tests.Application.Common;
using Bistrosoft.Orders.Tests.TestHelpers;
using Moq;
using Xunit;

namespace Bistrosoft.Orders.Tests.Application.Customers.CreateCustomer;

public class CreateCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateCustomerCommandHandler _handler;

    public CreateCustomerCommandHandlerTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateCustomerCommandHandler(
            _customerRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateCustomer_WhenDataIsValid()
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            Name = TestFixtures.Customers.ValidName,
            Email = TestFixtures.Customers.ValidEmail,
            PhoneNumber = TestFixtures.Customers.ValidPhone
        };

        _customerRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Customer?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _customerRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Domain.Entities.Customer>(c => 
                c.Name == command.Name && 
                c.Email.Value == command.Email), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenEmailAlreadyExists()
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            Name = TestFixtures.Customers.ValidName,
            Email = TestFixtures.Customers.ValidEmail,
            PhoneNumber = TestFixtures.Customers.ValidPhone
        };

        var existingCustomer = EntityBuilder.CreateCustomer(email: command.Email);
        
        _customerRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCustomer);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Theory]
    [InlineData("", "valid@email.com", "Name is required")]
    [InlineData("John", "", "Email is required")]
    [InlineData("John", "invalid-email", "Email format")]
    public async Task Handle_ShouldThrowValidationException_WhenInputIsInvalid(
        string name, string email, string expectedMessagePart)
    {
        // Arrange
        var command = new CreateCustomerCommand
        {
            Name = name,
            Email = email
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => 
            _handler.Handle(command, CancellationToken.None));
        
        Assert.Contains(expectedMessagePart, exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
