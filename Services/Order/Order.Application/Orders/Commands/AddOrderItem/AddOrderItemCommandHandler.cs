using Order.Application.Catalog.Interfaces;
using Order.Application.Interfaces;

namespace Order.Application.Orders.Commands.AddOrderItem;

public sealed class AddOrderItemCommandHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusRepository _statusRepository;
    private readonly ICatalogClient _catalogClient;

    public AddOrderItemCommandHandler(
        IOrderRepository orderRepository,
        IOrderStatusRepository statusRepository,
        ICatalogClient catalogClient)
    {
        _orderRepository = orderRepository;
        _statusRepository = statusRepository;
        _catalogClient = catalogClient;
    }

    public async Task HandleAsync(
        AddOrderItemCommand command,
        CancellationToken cancellationToken = default)
    {


        //call catalog service from order service
        var product = await _catalogClient.GetProductByIdAsync(
            command.ProductId,
            cancellationToken);

        if (product is null)
            throw new InvalidOperationException("Product not found.");

        if (product.StockQuantity < command.Quantity)
            throw new InvalidOperationException("Insufficient stock.");


        var order = await _orderRepository.GetByIdAsync(
            command.OrderId,
            cancellationToken);

        if (order is null)
            throw new InvalidOperationException(
                "Order not found.");

        var pendingStatus = await _statusRepository.GetByStatusAsync(
            "Pending",
            cancellationToken);

        if (pendingStatus is null)
            throw new InvalidOperationException(
                "The Pending order status was not found.");

        order.AddItem(
             command.ProductId,
             command.Quantity,
             product.Price, //from Product service
             pendingStatus.Id);

        await _orderRepository.SaveChangesAsync(cancellationToken);
    }
}