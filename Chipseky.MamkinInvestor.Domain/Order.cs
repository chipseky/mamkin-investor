namespace Chipseky.MamkinInvestor.Domain;

public class Order
{
    public DateTime CreatedAt { get; private set; }
    public decimal CoinsAmount { get; private set; }
    public string TradingPair { get; private set; }
    public OrderType OrderType { get; private set; }
    public decimal ForecastedPrice { get; private set; }
    
    private Order() { }

    public static Order CreateByOrder(string tradingPair, int coinsAmount, decimal forecastedPrice)
    {
        if (string.IsNullOrEmpty(tradingPair))
            throw new InvalidOperationException();
        
        if(coinsAmount <= 0)
            throw new InvalidOperationException();
        
        return new Order
        {
            TradingPair = tradingPair,
            CoinsAmount = coinsAmount,
            CreatedAt = DateTime.UtcNow,
            OrderType = OrderType.Buy,
            ForecastedPrice = forecastedPrice
        };
    }
    
    public static Order CreateSellOrder(string tradingPair, decimal coinsAmount, decimal forecastedPrice)
    {
        return new Order
        {
            TradingPair = tradingPair,
            CoinsAmount = coinsAmount,
            CreatedAt = DateTime.UtcNow,
            OrderType = OrderType.Sell,
            ForecastedPrice = forecastedPrice
        };
    }
}

public enum OrderType
{
    Buy,
    Sell
}