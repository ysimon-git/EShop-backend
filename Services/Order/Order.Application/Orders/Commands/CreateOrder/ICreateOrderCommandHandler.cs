namespace Order.Application.Orders.Commands.CreateOrder;

public interface ICreateOrderCommandHandler
{
    Task<CreateOrderResult> HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default);
}