using System.Text.Json.Serialization;

namespace Chipseky.MamkinInvestor.Domain;

public class TradeOrder
{
    [JsonInclude] // https://github.com/dotnet/runtime/issues/42399
    public string TradeOrderId { get; private set; }
    
    [JsonInclude]
    public DateTime CreatedAt { get; private set; }
    
    /// <summary>
    /// Actual coin price
    /// </summary>
    [JsonInclude]
    public decimal? ActualAveragePrice { get; private set; }
    
    [JsonInclude]
    public decimal Quantity { get; private set; }

    [JsonInclude]
    public decimal? QuantityFilled { get; private set; }
    
    [JsonInclude]
    public decimal? ExecutedFee { get; private set; }
    
    /// <summary>
    /// for example, we asked to buy BTC with 10 USDT, but the exchange fullfill only 9.97749349,
    /// ValueFilled will have 9.97749349
    /// ValueRemaining will have 0.02250651
    /// ActualAveragePrice will have 59745.47 for 1 BTC
    /// QuantityFilled will have 0.000167 BTC
    /// </summary>
    [JsonInclude]
    public decimal? ValueFilled { get; private set; }
    
    [JsonInclude]
    public decimal? ValueRemaining { get; private set; }
    
    [JsonInclude]
    public OrderStatus Status { get; private set; }
    
    [JsonInclude]
    public OrderSide OrderSide { get; private set; }
    
    [JsonConstructor] // DDD principles, forgive me, https://stackoverflow.com/a/73493858/6160271
    private TradeOrder(){}

    public static TradeOrder Create(
        string tradeOrderId, 
        decimal? actualAveragePrice,
        decimal quantity,
        decimal? quantityFilled,
        decimal? executedFee,
        decimal? valueFilled,
        decimal? valueRemaining,
        OrderStatus status,
        OrderSide orderSide)
    {
        return new TradeOrder
        {
            TradeOrderId = tradeOrderId,
            CreatedAt = DateTime.UtcNow,
            ActualAveragePrice = actualAveragePrice,
            Quantity = quantity,
            QuantityFilled = quantityFilled,
            ExecutedFee = executedFee,
            ValueFilled = valueFilled,
            ValueRemaining = valueRemaining,
            Status = status,
            OrderSide = orderSide
        };
    }
}

public enum OrderSide
{
    Buy,
    Sell
}

/// <summary>
/// Order status
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Created but not yet in matching engine
    /// </summary>
    Created,
    /// <summary>
    /// Placed successfully
    /// </summary>
    New,
    /// <summary>
    /// Rejected
    /// </summary>
    Rejected,
    /// <summary>
    /// Partially filled
    /// </summary>
    PartiallyFilled,
    /// <summary>
    /// Partially filled and cancelled
    /// </summary>
    PartiallyFilledCanceled,
    /// <summary>
    /// Filled
    /// </summary>
    Filled,
    /// <summary>
    /// Cancelled
    /// </summary>
    Cancelled,
    /// <summary>
    /// Untriggered
    /// </summary>
    Untriggered,
    /// <summary>
    /// Triggered
    /// </summary>
    Triggered,
    /// <summary>
    /// Deactivated
    /// </summary>
    Deactivated,
    /// <summary>
    /// Order has been triggered and the new active order has been successfully placed. Is the final state of a successful conditional order
    /// </summary>
    Active
}