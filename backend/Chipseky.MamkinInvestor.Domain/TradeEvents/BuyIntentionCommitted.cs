namespace Chipseky.MamkinInvestor.Domain.TradeEvents;

public class BuyIntentionCommitted : TradeEvent
{
    public decimal UsdtQuantity { get; private set; }
    public decimal ExpectedPrice { get; private set; }
    public string OrderId { get; private set; }
    
    private BuyIntentionCommitted() {}

    public BuyIntentionCommitted(Guid tradeId, string orderId, decimal usdtQuantity, decimal expectedPrice) 
        : base(tradeId)
    {
        OrderId = orderId;
        UsdtQuantity = usdtQuantity;
        ExpectedPrice = expectedPrice;
    }
}