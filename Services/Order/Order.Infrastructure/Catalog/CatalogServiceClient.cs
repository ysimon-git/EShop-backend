using System.Net.Http.Json;
using Order.Application.Catalog.Interfaces;
using Order.Application.Catalog.Models;

namespace Order.Infrastructure.Catalog;

public sealed class CatalogServiceClient : ICatalogServiceClient
{
    private readonly HttpClient _httpClient;

    public CatalogServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<CatalogProductDto>> GetProductsByIdsAsync(
        IEnumerable<Guid> productIds,
        CancellationToken cancellationToken = default)
    {
        var ids = productIds
            .Distinct()
            .ToList();

        if (ids.Count == 0)
            return [];

        var request = new
        {
            ProductIds = ids
        };

        var response = await _httpClient.PostAsJsonAsync(
            "api/products/by-ids",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var products =
            await response.Content.ReadFromJsonAsync<List<CatalogProductDto>>(
                cancellationToken: cancellationToken);

        return products ?? [];
    }
}