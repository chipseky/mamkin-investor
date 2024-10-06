namespace Chipseky.MamkinInvestor.Domain;

public class Trade
{
    public Guid TradeId { get; private set; }
    
    public string Symbol { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }
    
    public DateTime? ForecastedSellDate { get; private set; }
    
    public decimal HeldCoinsCount { get; private set; }

    public TradeState State { get; private set; }
    
    public Guid? ForecastId { get; private set; }
    
    public string? FailReason { get; private set; }
    
    private ICollection<TradeOrder> _history = new List<TradeOrder>();
    public IEnumerable<TradeOrder> History => _history;

    public decimal CurrentProfit { get; private set; }
    
    private Trade(){}

    public static Trade Create(Guid tradeId, string symbol, TimeSpan? sellOffset, Guid forecastId)
    {
        var currentMoment = DateTime.UtcNow;
        return new Trade
        {
            Symbol = symbol,
            TradeId = tradeId,
            CreatedAt = currentMoment,
            UpdatedAt = currentMoment,
            State = TradeState.Created,
            ForecastId = forecastId,
            ForecastedSellDate = sellOffset.HasValue ? DateTime.UtcNow + sellOffset.Value : null,
            CurrentProfit = 0
        };
    }

    public void Open(TradeOrder tradeOrder)
    {
        if (State != TradeState.Created)
            throw new InvalidOperationException();
        
        if(tradeOrder.OrderSide != OrderSide.Buy)
            throw new InvalidOperationException();
        
        State = TradeState.Opened;
        HeldCoinsCount = tradeOrder.QuantityFilled ?? 0;
        CurrentProfit = -CalcOpenOrderProfit(tradeOrder.ValueFilled);
        UpdatedAt = DateTime.UtcNow;
        _history.Add(tradeOrder);
    }

    private decimal CalcOpenOrderProfit(decimal? valueFilled)
    {
        if(!valueFilled.HasValue)
            return 0;
        
        return valueFilled.Value + valueFilled.Value * BybitCommissionConstants.BuyCommission;
    }
    
    private decimal CalcCloseOrderProfit(decimal? valueFilled)
    {
        if(!valueFilled.HasValue)
            return 0;
        
        return valueFilled.Value + valueFilled.Value * BybitCommissionConstants.SellCommission;
    }

    public void Close(TradeOrder tradeOrder)
    {
        if (State != TradeState.IsSold)
            throw new InvalidOperationException();
        
        if(tradeOrder.OrderSide != OrderSide.Sell)
            throw new InvalidOperationException();

        CurrentProfit += CalcCloseOrderProfit(tradeOrder.ValueFilled);
        State = TradeState.Closed;
        // HeldCoinsCount = tradeOrder.QuantityFilled!.Value;
        UpdatedAt = DateTime.UtcNow;
        _history.Add(tradeOrder);
    }
    
    public void MarkFailed(string eventType, string? reason, string? orderId)
    {
        if (State == TradeState.Closed)
            throw new InvalidOperationException();
        
        State = TradeState.Failed;
        UpdatedAt = DateTime.UtcNow;
        FailReason = $"Source: {eventType}. Order id: {orderId ?? "undefined"}. Reason: {reason ?? "undefined"}";
    }

    public void MarkSold()
    {
        if (State != TradeState.Opened)
            throw new InvalidOperationException();
        
        State = TradeState.IsSold;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum TradeState
{
    Created,
    Opened,
    IsSold,
    Closed,
    Failed
}