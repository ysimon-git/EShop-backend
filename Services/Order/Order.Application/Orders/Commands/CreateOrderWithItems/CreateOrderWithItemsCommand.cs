namespace Order.Application.Orders.Commands.CreateOrderWithItems;

public record CreateOrderWithItemsCommand(
    Guid CustomerId,
    string? Note,
    List<CreateOrderWithItemsItem> OrderItems
);

public record CreateOrderWithItemsItem(
    Guid ProductId,
    int Quantity
);