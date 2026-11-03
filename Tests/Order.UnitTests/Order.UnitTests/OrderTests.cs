
    
using Order.Domain.Entities;
using Order.Domain.Exceptions;
using Xunit;

namespace Order.UnitTests;

public class OrderTests
{
    private const int PendingStatusId = 1;
    private const int ConfirmedStatusId = 2;
    private const int CancelledStatusId = 3;

    private static Order.Domain.Entities.Order CreateOrder()
    {
        return new Order.Domain.Entities.Order(
            Guid.NewGuid(),
            PendingStatusId
        );
    }

    [Fact]
    public void Constructor_ShouldCreatePendingOrder()
    {
        // Arrange & Act
        var order = CreateOrder();

        // Assert
        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(PendingStatusId, order.StatusId);
        Assert.Empty(order.Items);
        Assert.Equal(0m, order.TotalAmount);
    }

    [Fact]
    public void AddItem_ShouldAddProductToOrder()
    {
        // Arrange
        var order = CreateOrder();
        var productId = Guid.NewGuid();

        // Act
        order.AddItem(productId, 2, 25m, PendingStatusId);

        // Assert
        Assert.Single(order.Items);
        Assert.Equal(productId, order.Items.First().ProductId);
    }

    [Fact]
    public void AddItem_ShouldThrow_WhenProductAlreadyExists()
    {
        // Arrange
        var order = CreateOrder();
        var productId = Guid.NewGuid();

        order.AddItem(productId, 1, 25m, PendingStatusId);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            order.AddItem(productId, 2, 25m, PendingStatusId)
        );

        Assert.Single(order.Items);
    }

    [Fact]
    public void TotalAmount_ShouldCalculateCorrectTotal()
    {
        // Arrange
        var order = CreateOrder();

        // Act
        order.AddItem(Guid.NewGuid(), 2, 25m, PendingStatusId);
        order.AddItem(Guid.NewGuid(), 3, 10m, PendingStatusId);

        // Assert: (2*25) + (3*10)
        Assert.Equal(80m, order.TotalAmount);
    }

    [Fact]
    public void Confirm_ShouldThrow_WhenOrderIsEmpty()
    {
        // Arrange
        var order = CreateOrder();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            order.Confirm(PendingStatusId, ConfirmedStatusId)
        );
    }

    [Fact]
    public void Confirm_ShouldChangeStatusToConfirmed()
    {
        // Arrange
        var order = CreateOrder();
        order.AddItem(Guid.NewGuid(), 1, 25m, PendingStatusId);

        // Act
        order.Confirm(PendingStatusId, ConfirmedStatusId);

        // Assert
        Assert.Equal(ConfirmedStatusId, order.StatusId);
    }

    [Fact]
    public void AddItem_ShouldThrow_WhenOrderIsConfirmed()
    {
        // Arrange
        var order = CreateOrder();

        order.AddItem(Guid.NewGuid(), 1, 25m, PendingStatusId);
        order.Confirm(PendingStatusId, ConfirmedStatusId);

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            order.AddItem(Guid.NewGuid(), 1, 10m, PendingStatusId)
        );
    }

    [Fact]
    public void Cancel_ShouldChangeStatusToCancelled()
    {
        // Arrange
        var order = CreateOrder();

        // Act
        order.Cancel(PendingStatusId, CancelledStatusId);

        // Assert
        Assert.Equal(CancelledStatusId, order.StatusId);
    }

    [Fact]
    public void SetOrderNumber_ShouldThrow_WhenEmpty()
    {
        // Arrange
        var order = CreateOrder();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            order.SetOrderNumber("")
        );
    }

    [Fact]
    public void ChangeNote_ShouldThrow_WhenTooLong()
    {
        // Arrange
        var order = CreateOrder();
        var note = new string('A', 501);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            order.ChangeNote(note)
        );
    }
}

