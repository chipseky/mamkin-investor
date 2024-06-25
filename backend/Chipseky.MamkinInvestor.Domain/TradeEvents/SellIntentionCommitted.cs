namespace Chipseky.MamkinInvestor.Domain.TradeEvents;

public class SellIntentionCommitted : TradeEvent
{
    public decimal ExpectedCoinsCount { get; private set; }
    public decimal ExpectedCoinPrice { get; private set; }
    
    public string OrderId { get; private set; }
    
    private SellIntentionCommitted() : base() {}

    public SellIntentionCommitted(Guid tradeId, string orderId, decimal expectedCoinsCount, decimal expectedCoinPrice) 
        : base(tradeId)
    {
        ExpectedCoinsCount = expectedCoinsCount;
        ExpectedCoinPrice = expectedCoinPrice;
        OrderId = orderId;
    }
}