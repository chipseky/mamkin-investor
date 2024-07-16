namespace Chipseky.MamkinInvestor.Domain;

public interface IPredefinedSymbolsRepository
{
    Task<ICollection<PredefinedSymbol>> Get();
    Task<TimeSpan?> GetForecastedSellOffset(string symbol);
}