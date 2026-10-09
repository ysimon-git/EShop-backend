using Order.Application.Catalog.Models;

namespace Order.Application.Catalog.Interfaces;

public interface ICatalogClient
{
    //call catalog service from Order service
    Task<CatalogProduct?> GetProductByIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
}