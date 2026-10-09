namespace Order.Application.Catalog.Models;

public sealed record CatalogProduct(
    Guid Id,
    decimal Price,
    int StockQuantity);