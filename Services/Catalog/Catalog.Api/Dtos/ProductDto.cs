namespace Catalog.Api.Dtos;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    string? ImageUrl,
    Guid CategoryId,
    string CategoryName,
    Guid BrandId,
    string BrandName
);