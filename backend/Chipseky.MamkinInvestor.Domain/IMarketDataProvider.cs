namespace Chipseky.MamkinInvestor.Domain;

public interface IMarketDataProvider
{
    Task<IEnumerable<MarketSymbol>> Get();
}

public record MarketSymbol(string Symbol, decimal LastPrice, decimal PriceChangePercentag24h);