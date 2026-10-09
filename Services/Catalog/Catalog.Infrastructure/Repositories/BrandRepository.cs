using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public sealed class BrandRepository : IBrandRepository
{
    private readonly CatalogDbContext _context;

    public BrandRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<List<Brand>> GetByIdsAsync(
    IEnumerable<Guid> ids,
    CancellationToken cancellationToken = default)
    {
        return await _context.Brands
            .AsNoTracking()
            .Where(b => ids.Contains(b.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Brand>> GetAllAsync()
    {
        return await _context.Brands
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        return _context.Brands.AnyAsync(c => c.Id == id);
    }

    public async Task AddAsync(Brand brand)
    {
        await _context.Brands.AddAsync(brand);
        await _context.SaveChangesAsync();
    }
}