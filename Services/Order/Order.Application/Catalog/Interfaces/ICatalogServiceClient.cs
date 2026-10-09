using Order.Application.Catalog.Models;

namespace Order.Application.Catalog.Interfaces;

public interface ICatalogServiceClient
{
    Task<IReadOnlyList<CatalogProductDto>> GetProductsByIdsAsync(
        IEnumerable<Guid> productIds,
        CancellationToken cancellationToken = default);
}