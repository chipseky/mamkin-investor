namespace Chipseky.MamkinInvestor.Domain.TradeEvents;

public class SellIntentionFailed : TradeEvent
{
    public string Reason { get; private set; }
    
    private SellIntentionFailed() {}

    public SellIntentionFailed(Guid tradeId, string reason): base(tradeId) 
    {
        Reason = reason;
    }
}