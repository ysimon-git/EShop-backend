using Catalog.Api.Dtos;
using Catalog.Domain.Entities;

namespace Catalog.Api.Mappers;

public static class ProductMapper
{
    public static ProductDto ToDto(Product product)
    {
        return new ProductDto(
            Id: product.Id,
            Name: product.Name,
            Description: product.Description,
            Price: product.Price,
            StockQuantity: product.StockQuantity,
            CategoryId: product.CategoryId,
            CategoryName: product.Category.Name,
            BrandId: product.BrandId,
            BrandName:product.Brand.Name,
            ImageUrl: product.ImageUrl
        );
    }
}

