using Order.Application.Interfaces;
using OrderEntity = Order.Domain.Entities.Order;


namespace Order.Application.Orders.Commands.CreateOrder;


public sealed class CreateOrderCommandHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusRepository _statusRepository;
    private readonly IOrderNumberGenerator _orderNumberGenerator;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IOrderStatusRepository statusRepository,
        IOrderNumberGenerator orderNumberGenerator)
        {
            _orderRepository = orderRepository;
            _statusRepository = statusRepository;
            _orderNumberGenerator = orderNumberGenerator;
        }


    public async Task<CreateOrderResult> HandleAsync(
    CreateOrderCommand command,
    CancellationToken cancellationToken = default)
    {
        var pendingStatus = await _statusRepository.GetByStatusAsync(
            "Pending",
            cancellationToken);

        if (pendingStatus is null)
            throw new InvalidOperationException(
                "The Pending order status was not found.");

        var orderNumber = await _orderNumberGenerator.GenerateAsync(
            cancellationToken);

        var order = new OrderEntity(
            command.CustomerId,
            pendingStatus.Id);

        order.SetOrderNumber(orderNumber);
        order.ChangeNote(command.Note);

        await _orderRepository.AddAsync(
            order,
            cancellationToken);

        return new CreateOrderResult(
            order.Id,
            order.OrderNumber);
    }
}