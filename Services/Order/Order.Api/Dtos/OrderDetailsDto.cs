

public sealed record OrderDetailsDto(
    Guid Id,
    string OrderNumber,
    Guid CustomerId,
    int StatusId,
    DateTime CreatedAtUtc,
    string? Note,
    decimal TotalAmount,
    IReadOnlyList<OrderItemDetailsDto> Items);