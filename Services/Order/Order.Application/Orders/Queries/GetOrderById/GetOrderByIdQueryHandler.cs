using Order.Application.Interfaces;

namespace Order.Application.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusRepository _statusRepository;

    public GetOrderByIdQueryHandler(
        IOrderRepository orderRepository,
        IOrderStatusRepository statusRepository)
    {
        _orderRepository = orderRepository;
        _statusRepository = statusRepository;
    }

    public async Task<OrderDetailsResult?> HandleAsync(
        GetOrderByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(
            query.OrderId,
            cancellationToken);

        if (order is null)
            return null;

        var status = await _statusRepository.GetByIdAsync(
            order.StatusId,
            cancellationToken);

        if (status is null)
            throw new InvalidOperationException(
                "The order status was not found.");

        var items = order.Items
            .Select(item => new OrderItemResult(
                item.ProductId,
                item.Quantity,
                item.UnitPrice,
                item.TotalPrice))
            .ToList();

        return new OrderDetailsResult(
            order.Id,
            order.CustomerId,
            status.Status,
            order.CreatedAtUtc,
            order.TotalAmount,
            items);
    }
}