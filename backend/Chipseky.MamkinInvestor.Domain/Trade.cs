namespace Chipseky.MamkinInvestor.Domain;

public class Trade
{
    public Guid TradeId { get; private set; }
    public string TradingPair { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }
    
    public decimal HeldCoinsCount { get; private set; }

    public bool Closed { get; private set; }
    
    private ICollection<TradeOrder> _history = new List<TradeOrder>();
    public IEnumerable<TradeOrder> History => _history;

    public decimal CurrentProfit { get; private set; }
    
    private Trade(){}

    public static Trade Create(Guid tradeId, string tradingPair)
    {
        var currentMoment = DateTime.UtcNow;
        return new Trade
        {
            TradingPair = tradingPair,
            TradeId = tradeId,
            CreatedAt = currentMoment,
            UpdatedAt = currentMoment,
            Closed = false,
            CurrentProfit = 0
        };
    }

    public void AddHistory(Guid tradeOrderId, OrderType orderType, decimal coinsCount, decimal actualPrice)
    {
        if(_history.Any(o => o.TradeOrderId == tradeOrderId))
            return;
        
        _history.Add(TradeOrder.Create(tradeOrderId, orderType, coinsCount, actualPrice));
        
        UpdatedAt = DateTime.UtcNow;
    }
}