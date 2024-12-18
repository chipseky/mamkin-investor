namespace Mamkin.In.Domain.TradeEvents;

public class BuyIntentionFailed : TradeEvent
{
    public string? Reason { get; private set; }
    
    public string? OrderId { get; private set; }
    
    // ReSharper disable once UnusedMember.Local
    private BuyIntentionFailed() {}

    public BuyIntentionFailed(Guid tradeId, string? orderId, string? reason): base(tradeId)
    {
        OrderId = orderId;
        Reason = reason;
    }
}