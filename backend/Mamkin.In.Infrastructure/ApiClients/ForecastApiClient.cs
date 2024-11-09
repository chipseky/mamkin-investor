using System.Text.Json;
using Mamkin.In.Domain;

namespace Mamkin.In.Infrastructure;

public class ForecastApiClient : IForecastApi
{
    private readonly HttpClient _httpClient;

    public ForecastApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Forecast> Get(string symbol)
    {
        using var response = await _httpClient.GetAsync($"/forecast?symbol={symbol}");

        var responseContent = await response.Content.ReadAsStringAsync();
        
        response.EnsureSuccessStatusCode();
        
        return JsonSerializer.Deserialize<Forecast>(responseContent)!;
    }
}