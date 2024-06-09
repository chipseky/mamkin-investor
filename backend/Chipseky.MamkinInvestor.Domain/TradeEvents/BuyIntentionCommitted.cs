namespace Chipseky.MamkinInvestor.Domain.TradeEvents;

public class BuyIntentionCommitted : TradeEvent
{
    public decimal CoinsCount { get; private set; }
    public decimal ActualPrice { get; private set; }
    
    private BuyIntentionCommitted() {}

    public BuyIntentionCommitted(Guid tradeId, decimal coinsCount, decimal actualPrice) : base(tradeId)
    {
        CoinsCount = coinsCount;
        ActualPrice = actualPrice;
    }
}