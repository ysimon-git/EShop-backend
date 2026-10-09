

public sealed record OrderItemDetailsDto(
    Guid ProductId,
    string ProductName,
    string? ImageUrl,
    string Category,
    string Brand,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);