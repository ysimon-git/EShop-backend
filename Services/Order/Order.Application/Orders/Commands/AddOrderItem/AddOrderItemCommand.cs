namespace Order.Application.Orders.Commands.AddOrderItem;
//command: write
public sealed record AddOrderItemCommand(
    Guid OrderId,
    Guid ProductId,
    int Quantity);
