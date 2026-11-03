namespace Order.Application.Orders.Queries.GetOrderById;

public sealed record OrderDetailsResult(
    Guid Id,
    string OrderNumber,
    Guid CustomerId,
    int StatusId,
    string Status,
    DateTime CreatedAtUtc,
    string? Note,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemResult> Items);



