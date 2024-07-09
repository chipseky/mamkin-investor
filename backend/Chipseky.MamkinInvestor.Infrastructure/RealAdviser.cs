using System.Text.Json;
using Chipseky.MamkinInvestor.Domain;

namespace Chipseky.MamkinInvestor.Infrastructure;

public class RealAdviser : IRealAdviser
{
    private readonly HttpClient _httpClient;

    public RealAdviser(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ShouldBuy(string symbol)
    {
        using var response = await _httpClient.GetAsync($"/api/advise/buy/{symbol}");

        var responseContent = await response.Content.ReadAsStringAsync();
        
        response.EnsureSuccessStatusCode();
        
        return JsonSerializer.Deserialize<bool>(responseContent);
    }

    public async Task<bool> ShouldSell(string symbol)
    {
        using var response = await _httpClient.GetAsync($"/api/advise/sell/{symbol}");

        var responseContent = await response.Content.ReadAsStringAsync();
        
        response.EnsureSuccessStatusCode();
        
        return JsonSerializer.Deserialize<bool>(responseContent);
    }
}