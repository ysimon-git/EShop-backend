namespace Order.Application.Interfaces;

public interface IOrderNumberGenerator
{
    Task<string> GenerateAsync(
        CancellationToken cancellationToken = default);
}