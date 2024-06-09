using Chipseky.MamkinInvestor.Domain.Repositories;
using Chipseky.MamkinInvestor.Domain.TradeEvents;

namespace Chipseky.MamkinInvestor.Domain;

public class OrdersManager
{
    private readonly IOrdersApi _ordersApi;
    private readonly ITradeEventsRepository _tradeEventsRepository;
    private readonly ITradeRepository _tradeRepository;
    
    public OrdersManager(IOrdersApi ordersApi, ITradeEventsRepository tradeEventsRepository, ITradeRepository tradeRepository)
    {
        _ordersApi = ordersApi;
        _tradeEventsRepository = tradeEventsRepository;
        _tradeRepository = tradeRepository;
    }

    public async Task<decimal> CalcProfit()
    {
        throw new NotImplementedException();
    }

    public async Task CreateBuyOrder(string tradingPair, int coinsCount, decimal expectedCoinPrice)
    {
        var tradeId = Guid.NewGuid();
        var buyIntentionCreatedEvent = new BuyIntentionCreated(tradingPair, tradeId, coinsCount, expectedCoinPrice);
        await _tradeEventsRepository.Store(buyIntentionCreatedEvent);
        
        var result = await _ordersApi.PlaceBuyOrder(tradingPair, coinsCount);

        if (result.Succeeded)
        {
            var buyIntentionCommittedEvent = new BuyIntentionCommitted(tradeId, result.CoinsCount, result.ActualPrice);
            await _tradeEventsRepository.Store(buyIntentionCommittedEvent);
        }
        else
        {
            var buyIntentionFailedEvent = new BuyIntentionFailed(tradeId, result.ErrorReason!);
            await _tradeEventsRepository.Store(buyIntentionFailedEvent);
        }
    }

    public async Task CreateSellOrder(string tradingPair, decimal expectedCoinPrice)
    {
        var trade = await _tradeRepository.GetCurrentTrade(tradingPair);

        if (trade == null)
            return;

        var sellIntentionEvent = new SellIntentionCreated(trade.TradeId, trade.HeldCoinsCount, expectedCoinPrice);
        await _tradeEventsRepository.Store(sellIntentionEvent);

        var result = await _ordersApi.PlaceSellOrder(trade.TradingPair, trade.HeldCoinsCount);

        if (result.Succeeded)
        {
            var buyIntentionCommittedEvent = new SellIntentionCommitted(trade.TradeId, result.CoinsCount, result.ActualPrice);
            await _tradeEventsRepository.Store(buyIntentionCommittedEvent);
        }
        else
        {
            var buyIntentionFailedEvent = new SellIntentionFailed(trade.TradeId, result.ErrorReason!);
            await _tradeEventsRepository.Store(buyIntentionFailedEvent);
        }
    }
}