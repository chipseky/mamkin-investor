using Chipseky.MamkinInvestor.Domain;

namespace Chipseky.MamkinInvestor.Infrastructure;

public class RealAdviser : IRealAdviser
{
    private readonly IForecastApi _forecastApi;

    public RealAdviser(IForecastApi forecastApi)
    {
        _forecastApi = forecastApi;
    }

    public async Task<bool> ShouldBuy(string symbol)
    {
        var forecast = await _forecastApi.Get(symbol);
        return forecast.HeightPriceProbability >= 60;
    }

    public async Task<bool> ShouldSell(string symbol)
    {
        // using var response = await _httpClient.GetAsync($"/api/advise/sell/{symbol}");
        //
        // var responseContent = await response.Content.ReadAsStringAsync();
        //
        // response.EnsureSuccessStatusCode();
        //
        // return JsonSerializer.Deserialize<bool>(responseContent);
        throw new NotImplementedException();
    }
}