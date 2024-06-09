namespace Chipseky.MamkinInvestor.Domain.TradeEvents;

public class BuyIntentionFailed : TradeEvent
{
    public string Reason { get; private set; }
    
    private BuyIntentionFailed() {}

    public BuyIntentionFailed(Guid tradeId, string reason): base(tradeId) 
    {
        Reason = reason;
    }
}