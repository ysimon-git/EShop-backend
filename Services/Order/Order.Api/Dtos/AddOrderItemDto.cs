namespace Order.Api.DTOs;

public sealed record AddOrderItemDto(
    Guid ProductId,
    int Quantity);
