using System.Text.Json;
using Chipseky.MamkinInvestor.Domain;

namespace Chipseky.MamkinInvestor.Infrastructure;

public class ForecastApi : IForecastApi
{
    private readonly HttpClient _httpClient;

    public ForecastApi(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Forecast> Get(string symbol)
    {
        using var response = await _httpClient.GetAsync($"/forecast?symbol={symbol}");

        var responseContent = await response.Content.ReadAsStringAsync();
        
        response.EnsureSuccessStatusCode();
        
        return JsonSerializer.Deserialize<Forecast>(responseContent);
    }
}