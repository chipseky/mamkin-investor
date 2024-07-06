namespace Chipseky.MamkinInvestor.Domain.Repositories;

public interface ITradesRepository
{
    Task<Trade> Get(Guid tradeId);
    Task<Trade?> GetActiveTrade(string tradingPair);
    Task<IEnumerable<Trade>> GetOpenTrades(CancellationToken cancellation);
    Task<decimal> GetProfit(CancellationToken cancellationToken);
    Task Save(Trade trade);
}