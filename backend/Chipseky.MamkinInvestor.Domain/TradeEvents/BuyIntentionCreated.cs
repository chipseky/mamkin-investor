namespace Chipseky.MamkinInvestor.Domain.TradeEvents;

public class BuyIntentionCreated : TradeEvent
{
    public string TradingPair { get; private set; }
    public int CoinsCount { get; private set; }
    public decimal ExpectedPrice { get; private set; }
    
    private BuyIntentionCreated() : base() {}

    public BuyIntentionCreated(
        string tradingPair,
        Guid tradeId, 
        int coinsCount, 
        decimal expectedPrice) : base(tradeId)
    {
        TradingPair = tradingPair;
        CoinsCount = coinsCount;
        ExpectedPrice = expectedPrice;
    }
}