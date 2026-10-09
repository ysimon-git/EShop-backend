using Catalog.Domain.Entities;

namespace Catalog.Application.Interfaces;

public interface IBrandRepository
{
    Task AddAsync(Brand brand);

    Task<bool> ExistsAsync(Guid id);

    Task<List<Brand>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Brand>> GetAllAsync();
}