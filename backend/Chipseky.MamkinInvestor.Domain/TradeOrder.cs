using System.Text.Json.Serialization;

namespace Chipseky.MamkinInvestor.Domain;

public class TradeOrder
{
    [JsonInclude] // https://github.com/dotnet/runtime/issues/42399
    public Guid TradeOrderId { get; private set; }
    
    [JsonInclude]
    public DateTime CreatedAt { get; private set; }
    
    [JsonInclude]
    public decimal CoinsCount { get; private set; }
    
    [JsonInclude]
    public decimal ActualPrice { get; private set; }

    [JsonInclude]
    public OrderType OrderType { get; private set; }
    
    [JsonConstructor] // DDD principles, forgive me, https://stackoverflow.com/a/73493858/6160271
    private TradeOrder(){}

    public static TradeOrder Create(Guid tradeOrderId, OrderType orderType, decimal coinsCount, decimal actualPrice)
    {
        return new TradeOrder
        {
            TradeOrderId = tradeOrderId,
            CreatedAt = DateTime.UtcNow,
            OrderType = orderType,
            CoinsCount = coinsCount,
            ActualPrice = actualPrice
        };
    }
}

public enum OrderType
{
    Buy,
    Sell
}