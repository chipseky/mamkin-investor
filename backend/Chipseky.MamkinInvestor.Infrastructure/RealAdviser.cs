using Chipseky.MamkinInvestor.Domain;

namespace Chipseky.MamkinInvestor.Infrastructure;

public class RealAdviser : IRealAdviser
{
    private readonly IForecastApi _forecastApi;

    public RealAdviser(IForecastApi forecastApi)
    {
        _forecastApi = forecastApi;
    }

    public async Task<(bool, Forecast)> ShouldBuy(string symbol)
    {
        var randomValue = new Random().Next(100);
        return (randomValue > 50, new Forecast(777, 779, 45, 55));
    }

    public async Task<bool> ShouldSell(string symbol)
    {
        var randomValue = new Random().Next(100);
        return randomValue > 50;
    }
}