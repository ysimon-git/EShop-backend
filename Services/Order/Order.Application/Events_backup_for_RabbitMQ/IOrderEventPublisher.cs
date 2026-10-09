namespace Order.Application.Events;

public interface IOrderEventPublisher
{
    Task PublishOrderConfirmedAsync(
        OrderConfirmedEvent orderConfirmedEvent,
        CancellationToken cancellationToken = default);
}