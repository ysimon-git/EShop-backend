namespace Order.Domain.Entities;

public class OrderStatus
{
    public int Id { get; private set; }
    public string Status { get; private set; } = string.Empty;

    // Constructeur pour EF Core
    private OrderStatus() { }

    public OrderStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Status is required.");

        Status = status.Trim();
    }
}