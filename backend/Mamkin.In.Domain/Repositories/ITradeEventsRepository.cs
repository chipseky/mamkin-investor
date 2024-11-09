using Mamkin.In.Domain.TradeEvents;

namespace Mamkin.In.Domain;

public interface ITradeEventsRepository
{
    Task Store(TradeEvent tradeEvent);
}