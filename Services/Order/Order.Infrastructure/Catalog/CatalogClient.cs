using System.Net;
using System.Net.Http.Json;
using Order.Application.Catalog.Interfaces;
using Order.Application.Catalog.Models;

namespace Order.Infrastructure.Catalog;

public sealed class CatalogClient : ICatalogClient
{
    private readonly HttpClient _httpClient;

    public CatalogClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    //call catalog service from Order service
    public async Task<CatalogProduct?> GetProductByIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"api/products/{productId}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CatalogProduct>(
            cancellationToken: cancellationToken);
    }
}