namespace Order.Application.Orders.Queries.GetOrderById;

public sealed record OrderDetailsResult(
    Guid Id,
    Guid CustomerId,
    string Status,
    DateTime CreatedAtUtc,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemResult> Items);