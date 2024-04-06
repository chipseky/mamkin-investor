namespace Chipseky.MamkinInvestor.Domain;

public class TradingPairsManager
{
    public Dictionary<string, TradingPairDetail> TradingPairs { get; } = new();

    public void Update(IDictionary<string, TradingPairPriceChange> marketTradingPairs)
    {
        if (marketTradingPairs == null || marketTradingPairs.Count == 0)
            throw new InvalidOperationException("You are trying to add empty market data");

        if (TradingPairs.Count == 0)
        {
            foreach (var tradingPairPriceChange in marketTradingPairs)
                TradingPairs.Add(tradingPairPriceChange.Key, new TradingPairDetail(tradingPairPriceChange.Value));
            
            return;
        }

        // existed trading pairs
        foreach (var tradingPair in TradingPairs)
        {
            if (marketTradingPairs.ContainsKey(tradingPair.Key))
                tradingPair.Value.Update(marketTradingPairs[tradingPair.Key]);
            else
                tradingPair.Value.MarkOutdated();
        }

        // new trading pairs
        foreach (var marketTradingPair in marketTradingPairs)
            if (!TradingPairs.ContainsKey(marketTradingPair.Key))
                TradingPairs.Add(marketTradingPair.Key, new TradingPairDetail(marketTradingPair.Value));
    }

    public void CleanUp()
    {
        foreach (var tradingPair in TradingPairs.ToList().Where(tradingPair => tradingPair.Value.Outdated))
            TradingPairs.Remove(tradingPair.Key);
    }
}