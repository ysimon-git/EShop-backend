

using Moq;
using Xunit;
using Order.Application.Catalog.Interfaces;
using Order.Application.Catalog.Models;
using Order.Application.Orders.Commands.CreateOrder;
using Order.Application.Orders.Commands.AddOrderItem;
using Order.Application.Orders.Commands.CreateOrderWithItems;

namespace Order.UnitTests.Application.Orders;

public class CreateOrderWithItemsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCreateOrderAddItemsAndRemoveStock()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        var expectedResult = new CreateOrderResult(
            orderId,
            "ORD-001"
        );

        // Mock the dependencies
        var createOrderMock = new Mock<ICreateOrderCommandHandler>();
        var addItemMock = new Mock<IAddOrderItemCommandHandler>();
        var catalogMock = new Mock<ICatalogClient>();

        // Simulate the creation of the initial order
        createOrderMock
            .Setup(x => x.HandleAsync(
                It.IsAny<CreateOrderCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Simulate adding items to the order
        addItemMock
            .Setup(x => x.HandleAsync(
                It.IsAny<AddOrderItemCommand>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Simulate stock removal in CatalogService
        catalogMock
            .Setup(x => x.RemoveProductFromStock(
                It.IsAny<CatalogProductStockChange>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Create the command containing two products
        var command = new CreateOrderWithItemsCommand(
            customerId,
            "Commande de test",
            new List<CreateOrderWithItemsItem>
            {
                new(productId1, 2),
                new(productId2, 3)
            }
        );

        // Create the handler with mocked dependencies
        var handler = new CreateOrderWithItemsHandler(
            createOrderMock.Object,
            addItemMock.Object,
            catalogMock.Object
        );

        // Act
        var result = await handler.HandleAsync(command);

        // Assert: verify the returned order
        Assert.Equal(orderId, result.Id);
        Assert.Equal("ORD-001", result.OrderNumber);

        // Verify that the initial order was created once
        createOrderMock.Verify(x => x.HandleAsync(
            It.Is<CreateOrderCommand>(
                c => c.CustomerId == customerId
                     && c.Note == "Commande de test"),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        // Verify that the first product was added
        addItemMock.Verify(x => x.HandleAsync(
            It.Is<AddOrderItemCommand>(
                c => c.ProductId == productId1
                     && c.Quantity == 2
                     && c.OrderId == orderId),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        // Verify that the second product was added
        addItemMock.Verify(x => x.HandleAsync(
            It.Is<AddOrderItemCommand>(
                c => c.ProductId == productId2
                     && c.Quantity == 3
                     && c.OrderId == orderId),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        // Verify stock removal for the first product
        catalogMock.Verify(x => x.RemoveProductFromStock(
            It.Is<CatalogProductStockChange>(
                p => p.Id == productId1
                     && p.StockQuantity == 2),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        // Verify stock removal for the second product
        catalogMock.Verify(x => x.RemoveProductFromStock(
            It.Is<CatalogProductStockChange>(
                p => p.Id == productId2
                     && p.StockQuantity == 3),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldNotRemoveStock_WhenAddItemFails()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var createOrderMock = new Mock<ICreateOrderCommandHandler>();
        var addItemMock = new Mock<IAddOrderItemCommandHandler>();
        var catalogMock = new Mock<ICatalogClient>();

        // Simulate successful creation of the initial order
        createOrderMock
            .Setup(x => x.HandleAsync(
                It.IsAny<CreateOrderCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new CreateOrderResult(orderId, "ORD-002")
            );

        // Simulate a failure when adding an item
        addItemMock
            .Setup(x => x.HandleAsync(
                It.IsAny<AddOrderItemCommand>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException("Cannot add item.")
            );

        var handler = new CreateOrderWithItemsHandler(
            createOrderMock.Object,
            addItemMock.Object,
            catalogMock.Object
        );

        var command = new CreateOrderWithItemsCommand(
            customerId,
            null,
            new List<CreateOrderWithItemsItem>
            {
                new(productId, 2)
            }
        );

        // Act & Assert: adding the item must throw
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(command)
        );

        // Verify that stock removal was never called
        // because adding the item failed
        catalogMock.Verify(
            x => x.RemoveProductFromStock(
                It.IsAny<CatalogProductStockChange>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}
