namespace Order.Application.Catalog.Models;

public sealed record CatalogProductDto(
    Guid Id,
    string Name,
    string? ImageUrl,
    string Category,
    string Brand);