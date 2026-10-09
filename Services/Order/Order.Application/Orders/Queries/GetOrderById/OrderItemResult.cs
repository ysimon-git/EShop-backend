namespace Order.Application.Orders.Queries.GetOrderById;

public sealed record OrderItemResult(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);