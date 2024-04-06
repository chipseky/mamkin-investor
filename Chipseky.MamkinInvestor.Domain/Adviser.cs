namespace Chipseky.MamkinInvestor.Domain;

public class Adviser
{
    public Advice GiveAdvice(TradingPairDetail tradingPairDetail)
    {
        if (tradingPairDetail is { Outdated: true, OnHold: true })
            return Advice.Sell;
        
        if (tradingPairDetail.Outdated)
            return Advice.DoNothing;

        if (tradingPairDetail.Trend.Count < 3)
            return Advice.DoNothing;

        var trendMap = new bool[tradingPairDetail.Trend.Count];
        tradingPairDetail.Trend.CopyTo(trendMap, 0);

        // price was increased three times in a row
        if (trendMap[^1] && trendMap[^2] && trendMap[^3])
            return tradingPairDetail.OnHold ? Advice.DoNothing : Advice.Buy;

        // price was decreased twice in a row
        if (!trendMap[^1] && !trendMap[^2])
            return tradingPairDetail.OnHold ? Advice.Sell : Advice.DoNothing;
        
        return Advice.DoNothing;
    }
}

public enum Advice
{
    Buy,
    Sell,
    DoNothing
}