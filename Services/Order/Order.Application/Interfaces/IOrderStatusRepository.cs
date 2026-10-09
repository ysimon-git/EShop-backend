using Order.Domain.Entities;

namespace Order.Application.Interfaces;

public interface IOrderStatusRepository
{
    Task<OrderStatus?> GetByStatusAsync(
        string status,
        CancellationToken cancellationToken = default);

    Task<OrderStatus?> GetByIdAsync(
    int id,
    CancellationToken cancellationToken = default);
}