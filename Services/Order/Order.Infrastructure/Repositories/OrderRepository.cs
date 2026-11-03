using Microsoft.EntityFrameworkCore;
using Order.Application.Interfaces;
using Order.Application.Orders.Queries.GetOrderByCustomerId;
using Order.Infrastructure.Persistence;
using OrderEntity = Order.Domain.Entities.Order;

namespace Order.Infrastructure.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        OrderEntity order,
        CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }


    public Task<OrderEntity?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        return _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }



    public async Task<List<OrderSummaryDto>> GetByCustomerIdAsync(
    Guid customerId,
    CancellationToken cancellationToken = default)
    {

        return await (
            from order in _context.Orders
            join status in _context.OrderStatus
                on order.StatusId equals status.Id
            where order.CustomerId == customerId
            orderby order.CreatedAtUtc descending
            select new OrderSummaryDto(
                order.Id,
                order.OrderNumber,
                order.CustomerId,
                order.CreatedAtUtc,
                status.Status,
                order.Items.Sum(item => item.Quantity * item.UnitPrice)
            )
        ).ToListAsync();
    }




    public Task SaveChangesAsync(
    CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }


    public async Task<bool> ChangeStatusAsync(
     Guid orderId,
     int targetStatusId,
     CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order is null)
            return false;

        var pending = await _context.OrderStatus
            .AsNoTracking()
            .FirstOrDefaultAsync(
                s => s.Status == "Pending",
                cancellationToken);

        var target = await _context.OrderStatus
            .AsNoTracking()
            .FirstOrDefaultAsync(
                s => s.Id == targetStatusId,
                cancellationToken);

        if (pending is null || target is null)
            return false;

        switch (target.Status)
        {
            case "Confirmed":
                order.Confirm(pending.Id, target.Id);
                break;

            case "Cancelled":
                order.Cancel(pending.Id, target.Id);
                break;

            default:
                return false;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

   
}
