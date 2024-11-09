using Mamkin.In.Domain;

namespace Mamkin.In.WebApi.Extensions;

public static class TradingPairsHelper
{
    public static string GetAsString(this Dictionary<string, SymbolPriceChange> tradingPairs)
    {
        var tradingPairsStrings = tradingPairs
            .Select(t
                => $"{t.Key} - {t.Value.PriceChangePercentage24H * 100}% - {t.Value.LastPrice}");

        var result = string.Join('\n', tradingPairsStrings);

        return result;
    }
}