namespace Order.Api.Dtos;

public sealed record CreateOrderDto(
    Guid CustomerId,
    string? Note);