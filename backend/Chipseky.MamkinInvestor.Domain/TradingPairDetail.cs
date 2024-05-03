namespace Chipseky.MamkinInvestor.Domain;

public class TradingPairDetail
{
    public bool OnHold { get; private set; }
    public bool Outdated { get; private set; }
    public TradingPairPriceChange LastPriceChange { get; private set; }

    public CircularQueue<bool> Trend { get; }
    private const byte TrendQueueSize = 5;

    public TradingPairDetail(TradingPairPriceChange tradingPairPriceChange)
    {
        Trend = new(TrendQueueSize);
        LastPriceChange = tradingPairPriceChange;
    }

    public void Update(TradingPairPriceChange tradingPairPriceChange)
    {
        if(LastPriceChange.PriceChangePercentage24H < tradingPairPriceChange.PriceChangePercentage24H)
            Trend.Enqueue(true);
        else
            Trend.Enqueue(false);

        LastPriceChange = tradingPairPriceChange;
    }

    public void MarkOutdated() => Outdated = true;
    public void MarkAsHeld() => OnHold = true;
    public void ResetHold() => OnHold = false;

}

// https://stackoverflow.com/questions/1292/limit-size-of-queuet-in-net
public class CircularQueue<T> : Queue<T>
{
    private readonly int _limit;

    public CircularQueue(int limit) : base(limit)
    {
        _limit = limit;
    }

    public new void Enqueue(T item)
    {
        while (Count >= _limit)
            Dequeue();
        
        base.Enqueue(item);
    }
}