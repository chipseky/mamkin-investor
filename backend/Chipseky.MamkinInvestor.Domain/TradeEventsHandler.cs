using Chipseky.MamkinInvestor.Domain.Repositories;
using Chipseky.MamkinInvestor.Domain.TradeEvents;
using Microsoft.Extensions.Logging;

namespace Chipseky.MamkinInvestor.Domain;

public class TradeEventsHandler
{
    private readonly ITradeRepository _tradeRepository;
    private readonly ILogger<TradeEventsHandler> _logger;

    public TradeEventsHandler(
        ITradeRepository tradeRepository, 
        ILogger<TradeEventsHandler> logger)
    {
        _tradeRepository = tradeRepository;
        _logger = logger;
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
                {
                    _logger.LogWarning("Trade with {id} not found.", committedEvent.TradeId);
                    PostponeEventHandling();
                }

                trade!.AddHistory(
                    tradeOrderId: committedEvent.TradeEventId,
                    orderType: OrderType.Buy, 
                    coinsCount: committedEvent.CoinsCount, 
                    actualPrice: committedEvent.ActualPrice);

                await _tradeRepository.Save(trade);
                break;
            }
            case SellIntentionCommitted committedEvent:
            {
                var trade = await _tradeRepository.Get(committedEvent.TradeId);
                if (trade == null)
                {
                    _logger.LogWarning("Trade with {id} not found. Event: {eventType}", 
                        committedEvent.TradeId, nameof(SellIntentionCommitted));
                    PostponeEventHandling();
                }
                
                trade!.AddHistory(
                    tradeOrderId: committedEvent.TradeEventId,
                    orderType: OrderType.Sell, 
                    coinsCount: committedEvent.CoinsCount, 
                    actualPrice: committedEvent.ActualPrice);

                await _tradeRepository.Save(trade);
                break;
            }
        }
        // else if ...
    }

    private void PostponeEventHandling()
    {
        throw new InvalidDataException("delay attempt");
    }
}