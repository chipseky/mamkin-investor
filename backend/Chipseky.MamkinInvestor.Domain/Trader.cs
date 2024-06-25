using Chipseky.MamkinInvestor.Domain.Repositories;

namespace Chipseky.MamkinInvestor.Domain;

public class Trader
{
    private readonly OrdersManager _ordersManager;

    private readonly TradingPairsManager _tradingPairsManager;
    private readonly ITradeRepository _tradeRepository;
    private readonly Adviser _adviser = new();

    public Trader(OrdersManager ordersManager, TradingPairsManager tradingPairsManager, ITradeRepository tradeRepository)
    {
        _ordersManager = ordersManager;
        _tradingPairsManager = tradingPairsManager;
        _tradeRepository = tradeRepository;
    }

    public async Task Feed(IDictionary<string, TradingPairPriceChange> marketData)
    {
        _tradingPairsManager.Update(marketData);

        foreach (var tradingPair in _tradingPairsManager.TradingPairs)
        {
            var advice = _adviser.GiveAdvice(tradingPair.Value);

            switch (advice)
            {
                case Advice.Buy:
                    var symbolAlreadyHeld = await _tradeRepository.GetCurrentTrade(tradingPair.Key) != null;
                    if (symbolAlreadyHeld)
                        return;

                    var usdtQuantity = GetUsdtQuantity(tradingPair.Key);
                    await _ordersManager.CreateBuyOrder(tradingPair.Key, usdtQuantity,
                        tradingPair.Value.LastPriceChange.LastPrice);
                    break;
                case Advice.Sell:
                    var dontHaveOpenedTrades = await _tradeRepository.GetCurrentTrade(tradingPair.Key) == null;
                    if (dontHaveOpenedTrades)
                        return;

                    await _ordersManager.CreateSellOrder(tradingPair.Key, tradingPair.Value.LastPriceChange.LastPrice);
                    break;
                case Advice.DoNothing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        _tradingPairsManager.CleanUp();
    }

    private int GetUsdtQuantity(string symbol)
    {
        //todo: use LotSizeFilterService 
        return 10;
    }
}