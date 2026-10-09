namespace Order.Application.Events;

public sealed record OrderConfirmedEvent(
    Guid OrderId,
    DateTime ConfirmedAtUtc
);