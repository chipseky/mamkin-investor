using Chipseky.MamkinInvestor.Domain.TradeEvents;

namespace Chipseky.MamkinInvestor.Domain;

public interface ITradeEventsRepository
{
    Task Store(TradeEvent tradeEvent);
}