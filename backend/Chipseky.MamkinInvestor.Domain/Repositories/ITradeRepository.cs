namespace Chipseky.MamkinInvestor.Domain.Repositories;

public interface ITradeRepository
{
    Task<Trade?> Get(Guid tradeId);
    Task<Trade?> GetCurrentTrade(string tradingPair);
    Task Save(Trade trade);
}