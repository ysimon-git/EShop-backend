using Order.Api.DTOs;

namespace Order.Api.Dtos;

public sealed record CreateOrderWithItemsDto(
    string? note,
    AddOrderItemDto[] items);


