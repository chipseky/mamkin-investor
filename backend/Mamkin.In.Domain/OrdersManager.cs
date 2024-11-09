using Mamkin.In.Domain.Repositories;
using Mamkin.In.Domain.TradeEvents;
using Microsoft.Extensions.Logging;

namespace Mamkin.In.Domain;

public class OrdersManager
{
    private readonly IOrdersApi _ordersApi;
    private readonly ITradeEventsRepository _tradeEventsRepository;
    private readonly ITradesRepository _tradesRepository;
    private readonly IPredefinedSymbolsRepository _predefinedSymbolsRepository;
    private readonly ILogger<OrdersManager> _logger;

    public OrdersManager(
        IOrdersApi ordersApi,
        ITradeEventsRepository tradeEventsRepository,
        ITradesRepository tradesRepository,
        ILogger<OrdersManager> logger, 
        IPredefinedSymbolsRepository predefinedSymbolsRepository)
    {
        _ordersApi = ordersApi;
        _tradeEventsRepository = tradeEventsRepository;
        _tradesRepository = tradesRepository;
        _logger = logger;
        _predefinedSymbolsRepository = predefinedSymbolsRepository;
    }

    public async Task CreateBuyOrder(string symbol, decimal usdtQuantity, decimal expectedCoinPrice, Guid forecastId)
    {
        var tradeId = Guid.NewGuid();

        try
        {
            var sellOffset = await _predefinedSymbolsRepository.GetForecastedSellOffset(symbol);
            await _tradesRepository.Save(Trade.Create(tradeId, symbol, sellOffset, forecastId));

            var buyIntentionCreatedEvent = new BuyIntentionCreated(symbol, tradeId, usdtQuantity, expectedCoinPrice);
            await _tradeEventsRepository.Store(buyIntentionCreatedEvent);

            //todo: create a job checks only created but never opened trades
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

    public async Task CreateSellOrder(Trade trade)
    {
        try
        {
            trade.MarkSold();
            await _tradesRepository.Save(trade);
            
            //todo: create a job checks only IsSold (with SellIntentionCreated events) but never Closed trades
            var sellIntentionEvent = new SellIntentionCreated(trade.TradeId, trade.HeldCoinsCount, trade.HeldCoinsCount);
            await _tradeEventsRepository.Store(sellIntentionEvent);

            var result = await _ordersApi.PlaceSellOrder(trade.Symbol, trade.HeldCoinsCount, sellIntentionEvent.TradeEventId.ToString());

            if (result.Succeeded)
            {
                var buyIntentionCommittedEvent = new SellIntentionCommitted(
                    tradeId: trade.TradeId, orderId: result.OrderId!, expectedCoinsCount: trade.HeldCoinsCount, trade.HeldCoinsCount);
                await _tradeEventsRepository.Store(buyIntentionCommittedEvent);
            }
            else
            {
                var buyIntentionFailedEvent = new SellIntentionFailed(trade.TradeId, result.OrderId, result.Error);
                await _tradeEventsRepository.Store(buyIntentionFailedEvent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(OrdersManager)}.{nameof(CreateSellOrder)} error. Symbol: {trade.Symbol}");

        }
    }
}