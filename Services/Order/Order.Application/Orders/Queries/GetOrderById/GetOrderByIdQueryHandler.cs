using Order.Application.Catalog.Interfaces;
using Order.Application.Interfaces;

namespace Order.Application.Orders.Queries.GetOrderById;


//Get Order with all items in one handler

public sealed class GetOrderByIdQueryHandler : IGetOrderByIdQueryHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusRepository _statusRepository;
    private readonly ICatalogServiceClient _catalogServiceClient;

    public GetOrderByIdQueryHandler(
        IOrderRepository orderRepository,
        IOrderStatusRepository statusRepository,
        ICatalogServiceClient catalogServiceClient)
    {
        _orderRepository = orderRepository;
        _statusRepository = statusRepository;
        _catalogServiceClient = catalogServiceClient;
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


        // Get all distinct ProductIds from the order.
        var productIds = order.Items
            .Select(item => item.ProductId)
            .Distinct()
            .ToList();

        // Only ONE HTTP request to CatalogService.
        var products = await _catalogServiceClient.GetProductsByIdsAsync(
            productIds,
            cancellationToken);

        var items = order.Items
            .Select(item =>
            {
                var product = products.First(p =>
                    p.Id == item.ProductId);

                return new OrderItemResult(
                    item.ProductId,
                    product.Name,
                    product.ImageUrl,
                    product.Category,
                    product.Brand,
                    item.Quantity,
                    item.UnitPrice,
                    item.TotalPrice);
            })
            .ToList();

        var result = new OrderDetailsResult(
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            order.StatusId,
            status.Status,
            order.CreatedAtUtc,
            order.Note,
            order.TotalAmount,
            items);

        return result;

  
    }
}