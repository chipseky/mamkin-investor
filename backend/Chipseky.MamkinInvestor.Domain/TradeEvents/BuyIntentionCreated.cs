namespace Chipseky.MamkinInvestor.Domain.TradeEvents;

public class BuyIntentionCreated : TradeEvent
{
    public string Symbol { get; private set; }
    public decimal UsdtQuantity { get; private set; }
    public decimal ExpectedPrice { get; private set; }
    
    private BuyIntentionCreated() : base() {}

    public BuyIntentionCreated(
        string symbol,
        Guid tradeId, 
        decimal usdtQuantity, 
        decimal expectedPrice) : base(tradeId)
    {
        Symbol = symbol;
        UsdtQuantity = usdtQuantity;
        ExpectedPrice = expectedPrice;
    }
}