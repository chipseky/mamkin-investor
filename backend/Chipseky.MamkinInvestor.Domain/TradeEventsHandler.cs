using Chipseky.MamkinInvestor.Domain.Repositories;
using Chipseky.MamkinInvestor.Domain.TradeEvents;
using Microsoft.Extensions.Logging;

namespace Chipseky.MamkinInvestor.Domain;

public class TradeEventsHandler
{
    private readonly ITradeRepository _tradeRepository;
    private readonly IOrdersApi _ordersApi;
    private readonly ILogger<TradeEventsHandler> _logger;

    public TradeEventsHandler(
        ITradeRepository tradeRepository, 
        ILogger<TradeEventsHandler> logger, IOrdersApi ordersApi)
    {
        _tradeRepository = tradeRepository;
        _logger = logger;
        _ordersApi = ordersApi;
    }

    public async Task Handle(object tradeEvent)
    {
        switch (tradeEvent)
        {
            case BuyIntentionCreated intentionEvent:
            {
                var trade = await _tradeRepository.Get(intentionEvent.TradeId);
                if (trade != null)
                    return;
            
                await _tradeRepository.Save(Trade.Create(intentionEvent.TradeId, intentionEvent.TradingPair));
                break;
            }
            case BuyIntentionCommitted committedEvent:
            {
                var trade = await _tradeRepository.Get(committedEvent.TradeId);
                if (trade == null)
                    PostponeEventHandling($"Trade with {committedEvent.TradeId} not found.");

                var tradeOrder = await _ordersApi.GetOrder(committedEvent.OrderId);
               
                //todo: order can be canceled
                trade!.Open(tradeOrder);

                await _tradeRepository.Save(trade);
                break;
            }
            case BuyIntentionFailed failedEvent:
            {
                var trade = await _tradeRepository.Get(failedEvent.TradeId);
                if (trade == null)
                    PostponeEventHandling($"Trade with {failedEvent.TradeId} not found.");

                trade!.MarkFailed(nameof(BuyIntentionFailed), failedEvent.Reason, failedEvent.OrderId);
                
                await _tradeRepository.Save(trade);
                break;
            }
            case SellIntentionCommitted committedEvent:
            {
                var trade = await _tradeRepository.Get(committedEvent.TradeId);
                if (trade == null)
                    PostponeEventHandling($"Trade with {committedEvent.TradeId} not found. Event: {nameof(SellIntentionCommitted)}");
                
                var tradeOrder = await _ordersApi.GetOrder(committedEvent.OrderId);
                
                trade!.Close(tradeOrder);

                await _tradeRepository.Save(trade);
                break;
            }
            case SellIntentionFailed failedEvent:
            {
                var trade = await _tradeRepository.Get(failedEvent.TradeId);
                if (trade == null)
                    PostponeEventHandling($"Trade with {failedEvent.TradeId} not found.");

                trade!.MarkFailed(nameof(SellIntentionFailed), failedEvent.Reason, failedEvent.OrderId);

                await _tradeRepository.Save(trade);
                break;
            }
        }
    }

    private void PostponeEventHandling(string reason)
    {
        throw new InvalidDataException($"Delay attempt. Reason: {reason}");
    }
}