namespace Chipseky.MamkinInvestor.Domain.TradeEvents;

public class SellIntentionCommitted : TradeEvent
{
    public decimal CoinsCount { get; private set; }
    public decimal ActualPrice { get; private set; }
    
    private SellIntentionCommitted() : base() {}

    public SellIntentionCommitted(Guid tradeId, decimal coinsCount, decimal actualPrice) : base(tradeId)
    {
        CoinsCount = coinsCount;
        ActualPrice = actualPrice;
    }
}