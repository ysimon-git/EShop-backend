using Catalog.Domain.Exceptions;

namespace Catalog.Domain.Entities;

public class Product
{
    //Product contains the business rules.
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public decimal Price { get; private set; }

    public int StockQuantity { get; private set; }

    public string? ImageUrl { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    public Guid BrandId { get; private set; }
    public Brand Brand { get; private set; } = null!;


    private Product()
    {
        // Required by EF Core.
    }

    public Product(
    string name,
    string description,
    decimal price,
    int stockQuantity,
    Guid categoryId,
    Guid brandId,
    string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required.");

        if (price <= 0)
            throw new DomainException(
                "Product price must be greater than zero.");

        if (stockQuantity < 0)
            throw new DomainException(
                "Stock quantity cannot be negative.");

        if (categoryId == Guid.Empty)
            throw new DomainException("Product category is required.");

        if (brandId == Guid.Empty)
            throw new DomainException("Product brand is required.");

        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        CategoryId = categoryId;
        BrandId = brandId;
        ImageUrl = imageUrl;
    }

    public void ChangePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new DomainException(
                "Product price must be greater than zero.");

        Price = newPrice;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException(
                "Quantity to add must be greater than zero.");

        StockQuantity += quantity;
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException(
                "Quantity to remove must be greater than zero.");

        if (quantity > StockQuantity)
            throw new DomainException(
                "Insufficient stock.");

        StockQuantity -= quantity;
    }

    public void ChangeImage(string? imageUrl)
    {
        ImageUrl = imageUrl;
    }
}