
using Order.Application.Catalog.Interfaces;
using Order.Application.Catalog.Models;
using Order.Application.Orders.Commands.AddOrderItem;
using Order.Application.Orders.Commands.CreateOrder;

namespace Order.Application.Orders.Commands.CreateOrderWithItems;

public sealed class CreateOrderWithItemsHandler
    : ICreateOrderWithItemsHandler
{
    private readonly ICreateOrderCommandHandler _createOrderCommandHandler;
    private readonly IAddOrderItemCommandHandler _addOrderItemCommandHandler;
    private readonly ICatalogClient _catalogClient;


    //call (once) createOrderCommandHandler and (for any item) addOrderItemCommandHandler

    public CreateOrderWithItemsHandler(
        ICreateOrderCommandHandler createOrderCommandHandler,
        IAddOrderItemCommandHandler addOrderItemCommandHandler,
        ICatalogClient catalogClient)
    {
        _createOrderCommandHandler = createOrderCommandHandler;
        _addOrderItemCommandHandler = addOrderItemCommandHandler;
        _catalogClient = catalogClient;
    }

    public async Task<CreateOrderWithItemsResult> HandleAsync(
        CreateOrderWithItemsCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Create the order
        var createCommand = new CreateOrderCommand(
            command.CustomerId,
            command.Note
        );

        var createOrderResult = await _createOrderCommandHandler.HandleAsync(
            createCommand,
            cancellationToken
        );

        // 2. Add each item to the order
        foreach (var item in command.OrderItems)
        {
            var addItemCommand = new AddOrderItemCommand(
                createOrderResult.Id,
                item.ProductId,
                item.Quantity
            );

            await _addOrderItemCommandHandler.HandleAsync(
                addItemCommand,
                cancellationToken
            );
        }

        // 3. Remove each item from stock
        foreach (var item in command.OrderItems)
        {
            var productStockQuantity = new CatalogProductStockChange(
                Id: item.ProductId,
                StockQuantity: item.Quantity
            );

            await _catalogClient.RemoveProductFromStock(
                productStockQuantity,
                cancellationToken
            );
        }

        // 4. Return the created order
        return new CreateOrderWithItemsResult(
            createOrderResult.Id,
            createOrderResult.OrderNumber
        );
    }
}