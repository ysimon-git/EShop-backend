using Microsoft.EntityFrameworkCore;
using Order.Application.Interfaces;
using Order.Domain.Entities;
using Order.Infrastructure.Persistence;

namespace Order.Infrastructure.Repositories;

public sealed class OrderStatusRepository : IOrderStatusRepository
{
    private readonly OrderDbContext _context;

    public OrderStatusRepository(OrderDbContext context)
    {
        _context = context;
    }

    public Task<OrderStatus?> GetByStatusAsync(
        string status,
        CancellationToken cancellationToken = default)
    {
        return _context.OrderStatus
            .AsNoTracking()
            .FirstOrDefaultAsync(
                s => s.Status == status,
                cancellationToken);
    }

    public Task<OrderStatus?> GetByIdAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        return _context.OrderStatus
            .AsNoTracking()
            .FirstOrDefaultAsync(
                status => status.Id == id,
                cancellationToken);
    }
}