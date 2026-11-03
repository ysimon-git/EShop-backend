namespace Order.Application.Orders.Commands.AddOrderItem;

public interface IAddOrderItemCommandHandler
{
    Task HandleAsync(
        AddOrderItemCommand command,
        CancellationToken cancellationToken = default);
}