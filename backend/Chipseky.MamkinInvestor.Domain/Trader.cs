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
            
            int coinsAmount;
            Trade? trade;
            
            switch (advice)
            {
                case Advice.Buy:
                    trade = await _tradeRepository.GetCurrentTrade(tradingPair.Key);
                    if (trade != null)
                        return;
                    
                    coinsAmount = GetCoinsAmount(tradingPair.Value.LastPriceChange.LastPrice);
                    
                    await _ordersManager.CreateBuyOrder(tradingPair.Key, coinsAmount, tradingPair.Value.LastPriceChange.LastPrice);

                    break;
                case Advice.Sell:
                    trade = await _tradeRepository.GetCurrentTrade(tradingPair.Key);
                    if (trade == null)
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

    private int GetCoinsAmount(decimal cointPrice)
    {
        var coinsAmount = 10;

        return cointPrice < 1
            ? coinsAmount * 1000
            : coinsAmount;
    }
}