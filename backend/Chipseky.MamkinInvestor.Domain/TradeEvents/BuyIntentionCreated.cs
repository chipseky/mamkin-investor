namespace Chipseky.MamkinInvestor.Domain.TradeEvents;

public class BuyIntentionCreated : TradeEvent
{
    public string TradingPair { get; private set; }
    public decimal UsdtQuantity { get; private set; }
    public decimal ExpectedPrice { get; private set; }
    
    private BuyIntentionCreated() : base() {}

    public BuyIntentionCreated(
        string tradingPair,
        Guid tradeId, 
        decimal usdtQuantity, 
        decimal expectedPrice) : base(tradeId)
    {
        TradingPair = tradingPair;
        UsdtQuantity = usdtQuantity;
        ExpectedPrice = expectedPrice;
    }
}