namespace Chipseky.MamkinInvestor.Domain.TradeEvents;

public abstract class TradeEvent
{
    public Guid TradeEventId { get; set; }
    public Guid TradeId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected TradeEvent() { }

    public TradeEvent(Guid tradeId)
    {
        TradeId = tradeId;
        TradeEventId = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}