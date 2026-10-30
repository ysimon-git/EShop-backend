
using Order.Application.Catalog.Interfaces;
using Order.Application.Catalog.Models;
using Order.Application.Orders.Commands.AddOrderItem;
using Order.Application.Orders.Commands.CreateOrder;

namespace Order.Application.Orders.Commands.CreateOrderWithItems;

public sealed class CreateOrderWithItemsHandler
{
    private readonly CreateOrderCommandHandler _createOrderHandler;
    private readonly AddOrderItemCommandHandler _addItemHandler;
    private readonly ICatalogClient _catalogClient;
    

    public CreateOrderWithItemsHandler(
        CreateOrderCommandHandler createOrderHandler,
        AddOrderItemCommandHandler addItemHandler,
        ICatalogClient catalogClient)
    {
        _createOrderHandler = createOrderHandler;
        _addItemHandler = addItemHandler;
        _catalogClient = catalogClient;
    }


    public async Task<CreateOrderResult> HandleAsync(
        CreateOrderWithItemsCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Create the order
        var createCommand = new CreateOrderCommand(
            command.CustomerId,
            command.Note
        );

        var result = await _createOrderHandler.HandleAsync(
            createCommand,
            cancellationToken
        );

        // 2. Add each item to the order
        foreach (var item in command.OrderItems)
        {
            var addItemCommand = new AddOrderItemCommand(
                result.Id,
                item.ProductId,
                item.Quantity
            );

            await _addItemHandler.HandleAsync(
                addItemCommand,
                cancellationToken
            );
        }

        // 3. Remove each item from the stock (call to catalog/products/RemoveStock endpoint )
        foreach(var item in command.OrderItems)
        {
            var productStockQuantity = new CatalogProductStockChange
            (
                Id:item.ProductId,
                StockQuantity: item.Quantity
            );



            await _catalogClient.RemoveProductFromStock(productStockQuantity);

        }



        // 4. Return the created order
        return result;
    }
}
