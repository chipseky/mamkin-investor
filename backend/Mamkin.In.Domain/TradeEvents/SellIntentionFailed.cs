namespace Mamkin.In.Domain.TradeEvents;

public class SellIntentionFailed : TradeEvent
{
    public string? Reason { get; private set; }
    
    public string? OrderId { get; private set; }
    
    private SellIntentionFailed() {}

    public SellIntentionFailed(Guid tradeId, string? orderId, string? reason): base(tradeId)
    {
        OrderId = orderId;
        Reason = reason;
    }
}