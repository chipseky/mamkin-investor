using Chipseky.MamkinInvestor.Domain;

namespace Chipseky.MamkinInvestor.WebApi.Extensions;

public static class TradingPairsHelper
{
    public static string GetAsString(this Dictionary<string, TradingPairPriceChange> tradingPairs)
    {
        var tradingPairsStrings = tradingPairs
            .Select(t
                => $"{t.Key} - {t.Value.PriceChangePercentage24H * 100}% - {t.Value.LastPrice}");

        var result = string.Join('\n', tradingPairsStrings);

        return result;
    }
}