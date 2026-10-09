using Catalog.Domain.Entities;

namespace Catalog.Application.Interfaces;

public interface ICategoryRepository
{
    Task AddAsync(Category category);

    Task<bool> ExistsAsync(Guid id);

    Task<List<Category>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Category>> GetAllAsync();
}