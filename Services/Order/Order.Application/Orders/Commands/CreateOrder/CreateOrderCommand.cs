namespace Order.Application.Orders.Commands.CreateOrder;
//command: write
public sealed record CreateOrderCommand(
    Guid CustomerId,
    string? Note);