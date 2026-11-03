using Order.Application.Orders.Queries.GetOrderByCustomerId;
using OrderEntity = Order.Domain.Entities.Order;

namespace Order.Application.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(
        OrderEntity order,
        CancellationToken cancellationToken = default);


    Task<OrderEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<OrderSummaryDto?>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);
    

    Task SaveChangesAsync(
    CancellationToken cancellationToken = default);

    Task<bool> ChangeStatusAsync(
    Guid orderId,
    int targetStatusId,
    CancellationToken cancellationToken = default);
}
