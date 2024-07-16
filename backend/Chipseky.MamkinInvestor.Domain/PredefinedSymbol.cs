namespace Chipseky.MamkinInvestor.Domain;

public class PredefinedSymbol
{
    public string Symbol { get; private set; }
    public TimeSpan? ForecastedSellOffset { get; private set; }

    public PredefinedSymbol(string symbol, TimeSpan? forecastedSellOffset)
    {
        Symbol = symbol;
        ForecastedSellOffset = forecastedSellOffset;
    }
}