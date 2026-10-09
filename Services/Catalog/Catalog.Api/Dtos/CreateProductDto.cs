namespace Catalog.Api.Dtos
{

    public sealed record CreateProductDto(
      string Name,
      string Description,
      decimal Price,
      int StockQuantity,
      Guid CategoryId,
      Guid BrandId,
      string? ImageUrl
  );
}
