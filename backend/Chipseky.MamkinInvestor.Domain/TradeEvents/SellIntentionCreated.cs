namespace Chipseky.MamkinInvestor.Domain.TradeEvents;

public class SellIntentionCreated : TradeEvent
{
    public decimal CoinsCount { get; private set; }
    public decimal ExpectedPrice { get; private set; }
    
    private SellIntentionCreated() : base() {}

    public SellIntentionCreated(Guid tradeId, decimal coinsCount, decimal expectedPrice) : base(tradeId)
    {
        CoinsCount = coinsCount;
        ExpectedPrice = expectedPrice;
    }
}