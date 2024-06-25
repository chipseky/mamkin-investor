using Chipseky.MamkinInvestor.Domain.Repositories;
using Chipseky.MamkinInvestor.Domain.TradeEvents;
using Microsoft.Extensions.Logging;

namespace Chipseky.MamkinInvestor.Domain;

public class OrdersManager
{
    private readonly IOrdersApi _ordersApi;
    private readonly ITradeEventsRepository _tradeEventsRepository;
    private readonly ITradeRepository _tradeRepository;
    private readonly ILogger<OrdersManager> _logger;
    
    public OrdersManager(
        IOrdersApi ordersApi, 
        ITradeEventsRepository tradeEventsRepository, 
        ITradeRepository tradeRepository, 
        ILogger<OrdersManager> logger)
    {
        _ordersApi = ordersApi;
        _tradeEventsRepository = tradeEventsRepository;
        _tradeRepository = tradeRepository;
        _logger = logger;
    }

    public async Task<decimal> CalcProfit()
    {
        throw new NotImplementedException();
    }

    public async Task CreateBuyOrder(string symbol, decimal usdtQuantity, decimal expectedCoinPrice)
    {
        var tradeId = Guid.NewGuid();

        try
        {
            var buyIntentionCreatedEvent = new BuyIntentionCreated(symbol, tradeId, usdtQuantity, expectedCoinPrice);
            await _tradeEventsRepository.Store(buyIntentionCreatedEvent);

            var result = await _ordersApi.PlaceBuyOrder(symbol, usdtQuantity, buyIntentionCreatedEvent.TradeEventId.ToString());

            if (result.Succeeded)
            {
                var buyIntentionCommittedEvent =
                    new BuyIntentionCommitted(tradeId, result.OrderId!, usdtQuantity, expectedCoinPrice);
                await _tradeEventsRepository.Store(buyIntentionCommittedEvent);
            }
            else
            {
                var buyIntentionFailedEvent = new BuyIntentionFailed(tradeId, orderId: result.OrderId, reason: result.Error);
                await _tradeEventsRepository.Store(buyIntentionFailedEvent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(OrdersManager)}.{nameof(CreateBuyOrder)} error. Trade id: {tradeId}");
        }
    }

    public async Task CreateSellOrder(string symbol, decimal expectedCoinPrice)
    {
        try
        {
            var trade = await _tradeRepository.GetCurrentTrade(symbol);

            if (trade == null)
                return;

            var sellIntentionEvent = new SellIntentionCreated(trade.TradeId, trade.HeldCoinsCount, expectedCoinPrice);
            await _tradeEventsRepository.Store(sellIntentionEvent);

            var result = await _ordersApi.PlaceSellOrder(trade.Symbol, trade.HeldCoinsCount, sellIntentionEvent.TradeEventId.ToString());

            if (result.Succeeded)
            {
                var buyIntentionCommittedEvent = new SellIntentionCommitted(
                    tradeId: trade.TradeId, orderId: result.OrderId!, expectedCoinsCount: trade.HeldCoinsCount, expectedCoinPrice);
                await _tradeEventsRepository.Store(buyIntentionCommittedEvent);
            }
            else
            {
                var buyIntentionFailedEvent = new SellIntentionFailed(trade.TradeId, result.OrderId, result.Error?.ToString());
                await _tradeEventsRepository.Store(buyIntentionFailedEvent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(OrdersManager)}.{nameof(CreateSellOrder)} error. Symbol: {symbol}");

        }
    }
}