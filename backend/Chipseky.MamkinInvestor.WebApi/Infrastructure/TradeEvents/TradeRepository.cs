using Chipseky.MamkinInvestor.Domain;
using Chipseky.MamkinInvestor.Domain.Repositories;
using Chipseky.MamkinInvestor.WebApi.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Chipseky.MamkinInvestor.WebApi.Infrastructure.TradeEvents;

public class TradeRepository : ITradeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TradeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Trade?> Get(Guid tradeId)
    {
        return await _dbContext.Trades
            .SingleOrDefaultAsync(t => t.TradeId == tradeId);
    }

    public async Task<Trade?> GetCurrentTrade(string tradingPair)
    {
        return await _dbContext.Trades
            .SingleOrDefaultAsync(t => t.TradingPair == tradingPair && t.Closed == false);
    }

    public async Task Save(Trade trade)
    {
        if (_dbContext.Entry(trade).State == EntityState.Detached)
            _dbContext.Trades.Add(trade);
        else
            // https://stackoverflow.com/a/62062499/6160271
            _dbContext.Entry(trade).Property(e => e.History).IsModified = true;

        await _dbContext.SaveChangesAsync();
    }
}