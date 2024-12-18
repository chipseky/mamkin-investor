using Mamkin.In.Domain.Repositories;

namespace Mamkin.In.Domain;

public class Trader
{
    private readonly OrdersManager _ordersManager;

    private readonly ITradesRepository _tradesRepository;
    private readonly IRealAdviser _realAdviser;
    private readonly IForecastsRepository _forecastsRepository;

    public Trader(OrdersManager ordersManager, ITradesRepository tradesRepository, IRealAdviser realAdviser, IForecastsRepository forecastsRepository)
    {
        _ordersManager = ordersManager;
        _tradesRepository = tradesRepository;
        _realAdviser = realAdviser;
        _forecastsRepository = forecastsRepository;
    }

    public async Task Feed(IDictionary<string, SymbolPriceChange> marketData)
    {
        foreach (var symbol in marketData)
        {
            var openedTrade = await _tradesRepository.GetActiveTrade(symbol.Key);
            if (openedTrade != null)
                continue;
            
            var (shouldBuy, forecast) = await _realAdviser.ShouldBuy(symbol.Key);
            await _forecastsRepository.Store(forecast);

            if (!shouldBuy) continue;

            var usdtQuantity = GetUsdtQuantity(symbol.Key);
            await _ordersManager.CreateBuyOrder(
                symbol:symbol.Key, 
                usdtQuantity: usdtQuantity,
                expectedCoinPrice: symbol.Value.LastPrice,
                forecastId: forecast.ForecastId);
        }
    }

    // ReSharper disable once UnusedParameter.Local
    private int GetUsdtQuantity(string symbol)
    {
        //todo: use LotSizeFilterService 
        return 10;
    }
}