namespace Order.Domain.Entities;

public sealed class OrderCounter
{
    public DateOnly Date { get; private set; }
    public int LastNumber { get; private set; }

    private OrderCounter() { }

    public OrderCounter(DateOnly date)
    {
        Date = date;
        LastNumber = 0;
    }

    public int GetNextNumber()
    {
        LastNumber++;
        return LastNumber;
    }
}