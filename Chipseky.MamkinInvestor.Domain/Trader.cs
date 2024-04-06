using Microsoft.Extensions.Logging;

namespace Chipseky.MamkinInvestor.Domain;

public class Trader
{
    private readonly OrdersManager _ordersManager;
    private readonly ILogger<Trader> _logger;

    private readonly TradingPairsManager _tradingPairsManager = new();
    private readonly Adviser _adviser = new();

    public Trader(OrdersManager ordersManager, ILogger<Trader> logger)
    {
        _ordersManager = ordersManager;
        _logger = logger;
    }

    public async Task Trade(IDictionary<string, TradingPairPriceChange> marketData)
    {
        _tradingPairsManager.Update(marketData);
        
        foreach (var tradingPair in _tradingPairsManager.TradingPairs)
        {
            var advice = _adviser.GiveAdvice(tradingPair.Value);
            
            int coinsAmount;
            decimal forecastedPrice;
            
            switch (advice)
            {
                case Advice.Buy:
                    (coinsAmount, forecastedPrice) = GetOrderValues(tradingPair.Value.LastPriceChange.LastPrice);

                    _logger.LogInformation($"Buy {tradingPair.Key}, amount: {coinsAmount}");

                    await _ordersManager.CreateByOrder(tradingPair.Key, coinsAmount, forecastedPrice);
                    tradingPair.Value.MarkAsHeld();

                    _logger.LogInformation($"{tradingPair.Key} has been bought, amount: {coinsAmount}");

                    break;
                case Advice.Sell:
                    (coinsAmount, forecastedPrice) = GetOrderValues(tradingPair.Value.LastPriceChange.LastPrice);

                    _logger.LogInformation($"Sell {tradingPair.Key}, amount: {coinsAmount}");

                    await _ordersManager.CreateSellOrder(tradingPair.Key, coinsAmount, forecastedPrice);
                    tradingPair.Value.ResetHold();

                    _logger.LogInformation($"{tradingPair.Key} has been sold , amount: {coinsAmount}");
                    break;
                case Advice.DoNothing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        _tradingPairsManager.CleanUp();
    }

    private (int coinsAmount, decimal forecastedPrice) GetOrderValues(decimal lastPrice)
    {
        var coinsAmount = 10;

        coinsAmount = lastPrice < 1
            ? coinsAmount * 1000
            : coinsAmount;
        var forecastedPrice = coinsAmount * lastPrice;

        return (coinsAmount, forecastedPrice);
    }
}