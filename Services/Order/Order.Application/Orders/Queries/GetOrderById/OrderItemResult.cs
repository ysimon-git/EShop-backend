namespace Order.Application.Orders.Queries.GetOrderById;

public sealed record OrderItemResult(
    Guid ProductId,
    string ProductName,
    string? ImageUrl,
    string Category,
    string Brand,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);

