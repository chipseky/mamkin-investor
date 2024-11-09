using Mamkin.In.Domain.Repositories;
using Mamkin.In.Domain.TradeEvents;
using Microsoft.Extensions.Logging;

namespace Mamkin.In.Domain;

public class TradeEventsHandler
{
    private readonly ITradesRepository _tradesRepository;
    private readonly IOrdersApi _ordersApi;
    private readonly ILogger<TradeEventsHandler> _logger;

    public TradeEventsHandler(
        ITradesRepository tradesRepository, 
        IOrdersApi ordersApi, 
        ILogger<TradeEventsHandler> logger)
    {
        _tradesRepository = tradesRepository;
        _ordersApi = ordersApi;
        _logger = logger;
    }

    public async Task Handle(object tradeEvent)
    {
        switch (tradeEvent)
        {
            case BuyIntentionCommitted committedEvent:
            {
                var trade = await _tradesRepository.Get(committedEvent.TradeId);

                if (trade.State != TradeState.Created)
                {
                    _logger.LogWarning($"Warning! Trade id {trade.TradeId}, trade status {trade.State}, {nameof(BuyIntentionCommitted)}");
                    break;
                }

                var tradeOrder = await _ordersApi.GetOrder(committedEvent.OrderId);
                
                if(tradeOrder == null)
                    trade.MarkFailed(
                        eventType: nameof(SellIntentionCommitted), 
                        reason: null,
                        orderId: committedEvent.OrderId);
                else
                    //todo: order can be canceled
                    trade.Open(tradeOrder);

                await _tradesRepository.Save(trade);
                break;
            }
            case BuyIntentionFailed failedEvent:
            {
                var trade = await _tradesRepository.Get(failedEvent.TradeId);

                trade.MarkFailed(nameof(BuyIntentionFailed), failedEvent.Reason, failedEvent.OrderId);
                
                await _tradesRepository.Save(trade);
                break;
            }
            case SellIntentionCommitted committedEvent:
            {
                var trade = await _tradesRepository.Get(committedEvent.TradeId);
                
                var tradeOrder = await _ordersApi.GetOrder(committedEvent.OrderId);

                if (trade.State!= TradeState.IsSold)
                {
                    _logger.LogWarning($"Warning! Trade id {trade.TradeId}, trade status {trade.State}, {nameof(SellIntentionCommitted)}");
                    break;
                }
                
                if(tradeOrder == null)
                    trade.MarkFailed(
                        eventType: nameof(SellIntentionCommitted), 
                        reason: null,
                        orderId: committedEvent.OrderId);
                else
                    trade.Close(tradeOrder);

                await _tradesRepository.Save(trade);
                break;
            }
            case SellIntentionFailed failedEvent:
            {
                var trade = await _tradesRepository.Get(failedEvent.TradeId);

                trade.MarkFailed(nameof(SellIntentionFailed), failedEvent.Reason, failedEvent.OrderId);

                await _tradesRepository.Save(trade);
                break;
            }
        }
    }
}