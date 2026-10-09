using Order.Domain.Exceptions;

namespace Order.Domain.Entities;

public class Order
{
    private readonly List<OrderItem> _items = new();

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public int StatusId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public decimal TotalAmount => _items.Sum(item => item.TotalPrice);

    public string OrderNumber { get; private set; } = string.Empty;

    public string? Note { get; private set; }


    // EF Core
    private Order() { }

    public Order(Guid customerId, int pendingStatusId)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId is required.");

        if (pendingStatusId <= 0)
            throw new ArgumentException("Pending status ID must be positive.");

        Id = Guid.NewGuid();
        CustomerId = customerId;
        StatusId = pendingStatusId;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void SetOrderNumber(string orderNumber)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new ArgumentException(
                "Order number is required.");

        OrderNumber = orderNumber;
    }

    public void ChangeNote(string? note)
    {
        if (note?.Length > 500)
            throw new ArgumentException(
                "Order note cannot exceed 500 characters.");

        Note = string.IsNullOrWhiteSpace(note)
            ? null
            : note.Trim();
    }

    public void AddItem(
        Guid productId,
        int quantity,
        decimal unitPrice,
        int pendingStatusId)
    {
        EnsurePending(pendingStatusId);

        if (_items.Any(item => item.ProductId == productId))
            throw new InvalidOperationException(
                "This product is already in the order.");

        _items.Add(new OrderItem(productId, quantity, unitPrice));
    }

    public void Confirm(int pendingStatusId, int confirmedStatusId)
    {
        EnsurePending(pendingStatusId);

        if (!_items.Any())
            throw new InvalidOperationException(
                "An empty order cannot be confirmed.");

        if (confirmedStatusId <= 0 || confirmedStatusId == pendingStatusId)
            throw new ArgumentException("Invalid confirmed status ID.");

        StatusId = confirmedStatusId;
    }

    public void Cancel(int pendingStatusId, int cancelledStatusId)
    {
        EnsurePending(pendingStatusId);

        if (cancelledStatusId <= 0 || cancelledStatusId == pendingStatusId)
            throw new ArgumentException("Invalid cancelled status ID.");

        StatusId = cancelledStatusId;
    }

    private void EnsurePending(int pendingStatusId)
    {
        if (pendingStatusId <= 0)
            throw new ArgumentException("Invalid pending status ID.");

       
        if (StatusId != pendingStatusId)
        {
            throw new DomainException(
                "Items can only be added to a pending order.");
        }
    }
}