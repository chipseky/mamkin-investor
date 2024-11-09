using Mamkin.In.Domain;
using Mamkin.In.Domain.Repositories;
using Mamkin.In.Infrastructure.Ef;
using Microsoft.EntityFrameworkCore;

namespace Mamkin.In.Infrastructure.DbRepositories;

public class TradesRepository : ITradesRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TradesRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Trade> Get(Guid tradeId)
    {
        //todo: remove it, https://github.com/npgsql/efcore.pg/issues/3220
        _dbContext.ChangeTracker.Clear();
        
        return await _dbContext.Trades
            .SingleAsync(t => t.TradeId == tradeId);
    }

    public async Task<Trade?> GetActiveTrade(string tradingPair)
    {
        return await _dbContext.Trades
            .SingleOrDefaultAsync(t => t.Symbol == tradingPair 
                                       && (t.State == TradeState.Opened || t.State == TradeState.Created));
    }

    public async Task<IEnumerable<Trade>> GetOpenTrades(CancellationToken cancellation)
    {
        //todo: create index
        var result = await _dbContext.Trades
            .Where(t => t.State == TradeState.Opened)
            .ToListAsync(cancellation);
        
        return result;
    }

    public async Task<decimal> GetProfit(CancellationToken cancellationToken)
    {
        // https://stackoverflow.com/questions/21521820/database-sqlqueryof-decimalsql-tostring-fails-if-no-results-from-query
        // I use as "Value" because of this: https://github.com/dotnet/efcore/issues/30447#issuecomment-1462231912

        var result = await _dbContext.Database
            .SqlQuery<decimal>($"SELECT cast(COALESCE(sum(t.current_profit), 0.0) as numeric(19,4)) as \"Value\" FROM trades AS t WHERE t.state = 'Closed'")
            .FirstAsync(cancellationToken);

        return result;
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