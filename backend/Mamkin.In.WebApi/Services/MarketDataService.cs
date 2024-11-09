using Mamkin.In.Domain;

namespace Mamkin.In.WebApi.Services;

public class MarketDataService
{
    private readonly IPredefinedSymbolsRepository _predefinedSymbolsRepository;
    private readonly IMarketDataProvider _marketDataProvider;

    public MarketDataService(
        IPredefinedSymbolsRepository predefinedSymbolsRepository, 
        IMarketDataProvider marketDataProvider)
    {
        _predefinedSymbolsRepository = predefinedSymbolsRepository;
        _marketDataProvider = marketDataProvider;
    }
    
    public async Task<Dictionary<string, SymbolPriceChange>> Get10HotSymbols()
    {
        var top10HotTicker = (await _marketDataProvider.Get())
            .OrderByDescending(t => t.PriceChangePercentag24h)
            .Take(10)
            .ToDictionary(
                t => t.Symbol,
                t => new SymbolPriceChange(
                    PriceChangePercentage24H: t.PriceChangePercentag24h,
                    LastPrice: t.LastPrice));

        return top10HotTicker;
    }
    
    public async Task<Dictionary<string, SymbolPriceChange>> GetSymbolsForTrading()
    {
        var predefinedSymbols = await _predefinedSymbolsRepository.Get();

        if (predefinedSymbols.Any())
        {
            var symbols = predefinedSymbols.Select(ps => ps.Symbol);
            return (await _marketDataProvider.Get())
                .Where(i => symbols.Contains(i.Symbol)).ToDictionary(
                    t => t.Symbol,
                    t => new SymbolPriceChange(
                        PriceChangePercentage24H: t.PriceChangePercentag24h,
                        LastPrice: t.LastPrice));
        }

        var top10HotTicker = (await _marketDataProvider.Get())
            .OrderByDescending(t => t.PriceChangePercentag24h)
            .Take(10)
            .ToDictionary(
                t => t.Symbol,
                t => new SymbolPriceChange(
                    PriceChangePercentage24H: t.PriceChangePercentag24h,
                    LastPrice: t.LastPrice));

        return top10HotTicker;
    }
}