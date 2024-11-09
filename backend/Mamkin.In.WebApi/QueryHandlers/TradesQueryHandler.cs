using Mamkin.In.Domain;
using Mamkin.In.Infrastructure.Ef;
using Mamkin.In.WebApi.Contracts.Paging;
using Mamkin.In.WebApi.Contracts.Req;
using Mamkin.In.WebApi.Contracts.Resp;
using Mamkin.In.WebApi.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Mamkin.In.WebApi.QueryHandlers;

public class TradesQueryHandler
{
    private readonly ApplicationDbContext _dbContext;

    public TradesQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IPagedData<TradesTableItem>> Handle(TradesQuery query)
    {
        var dbQuery = _dbContext.Trades.AsNoTracking();

        if (!string.IsNullOrEmpty(query.TradingPair))
            dbQuery = dbQuery.Where(o => o.Symbol == query.TradingPair);

        dbQuery = query.TradeState switch
        {
            TradeState.Created => dbQuery.Where(o => o.State == TradeState.Created),
            TradeState.Closed => dbQuery.Where(o => o.State == TradeState.Closed),
            TradeState.Opened => dbQuery.Where(o => o.State == TradeState.Opened),
            TradeState.Failed => dbQuery.Where(o => o.State == TradeState.Failed),
            _ => dbQuery
        };

        var total = await dbQuery.LongCountAsync();
        var trades = await dbQuery
            .OrderByDescending(t => t.CreatedAt)
            .Skip(query.Page * query.PageSize)
            .Take(query.PageSize)
            .Select(t => new TradesTableItem
            {
                TradeId = t.TradeId,
                TradingPair = t.Symbol,
                CreatedAt = t.CreatedAt,
                HeldCoinsCount = t.HeldCoinsCount,
                CurrentProfit = t.CurrentProfit,
                State = t.State,
                Orders = t.History
            })
            .ToListAsync();

        return new PagedData<TradesTableItem>(trades, query.PageSize, query.PageSize, total);
    }
}