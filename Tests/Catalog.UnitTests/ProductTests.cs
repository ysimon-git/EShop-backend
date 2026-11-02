
using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using Xunit;

namespace Catalog.UnitTests;

public class ProductTests
{
    // Creates a valid product for our tests.
    private static Product CreateProduct(int stockQuantity = 10)
    {
        return new Product(
            name: "Test Product",
            description: "Test Description",
            price: 25.00m,
            stockQuantity: stockQuantity,
            categoryId: Guid.NewGuid(),
            brandId: Guid.NewGuid()
        );
    }

    [Fact]
    public void RemoveStock_ShouldDecreaseStock()
    {
        // Arrange
        var product = CreateProduct(10);

        // Act
        product.RemoveStock(3);

        // Assert
        Assert.Equal(7, product.StockQuantity);
    }

    [Fact]
    public void RemoveStock_ShouldThrow_WhenInsufficientStock()
    {
        // Arrange
        var product = CreateProduct(10);

        // Act & Assert
        Assert.Throws<DomainException>(
            () => product.RemoveStock(15)
        );

        Assert.Equal(10, product.StockQuantity);
    }

    [Fact]
    public void RemoveStock_ShouldThrow_WhenQuantityIsZero()
    {
        // Arrange
        var product = CreateProduct(10);

        // Act & Assert
        Assert.Throws<DomainException>(
            () => product.RemoveStock(0)
        );
    }

    [Fact]
    public void AddStock_ShouldIncreaseStock()
    {
        // Arrange
        var product = CreateProduct(10);

        // Act
        product.AddStock(5);

        // Assert
        Assert.Equal(15, product.StockQuantity);
    }

    [Fact]
    public void ChangePrice_ShouldThrow_WhenPriceIsNegative()
    {
        // Arrange
        var product = CreateProduct();

        // Act & Assert
        Assert.Throws<DomainException>(
            () => product.ChangePrice(-10)
        );

        Assert.Equal(25.00m, product.Price);
    }
}
