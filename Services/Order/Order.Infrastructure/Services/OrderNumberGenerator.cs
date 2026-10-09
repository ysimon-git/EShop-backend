using Microsoft.EntityFrameworkCore;
using Order.Application.Interfaces;
using Order.Domain.Entities;
using Order.Infrastructure.Persistence;
using System.Data;

namespace Order.Infrastructure.Services;

public sealed class OrderNumberGenerator : IOrderNumberGenerator
{
    private readonly OrderDbContext _context;

    public OrderNumberGenerator(OrderDbContext context)
    {
        _context = context;
    }

    //be sure to generate unique number -> BeginTransactionAsync/CommitAsync
    public async Task<string> GenerateAsync(
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

        var counter = await _context.OrderCounters
            .FirstOrDefaultAsync(
                c => c.Date == today,
                cancellationToken);

        if (counter is null)
        {
            counter = new OrderCounter(today);

            _context.OrderCounters.Add(counter);
        }

        var nextNumber = counter.GetNextNumber();

        await _context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return $"{today:yyyyMMdd}-{nextNumber:D6}";
    }
}