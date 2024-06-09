using Chipseky.MamkinInvestor.WebApi.Infrastructure;
using Chipseky.MamkinInvestor.WebApi.Infrastructure.Database;
using Chipseky.MamkinInvestor.WebApi.Queries;
using Microsoft.EntityFrameworkCore;

namespace Chipseky.MamkinInvestor.WebApi.QueryHandlers;

public class TradesTableDataQueryHandler
{
    private readonly ApplicationDbContext _dbContext;

    public TradesTableDataQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IPagedData<TradesTableItem>> Handle(TradesTableDataQuery query)
    {
        var dbQuery = _dbContext.Trades.AsNoTracking();

        if (!string.IsNullOrEmpty(query.TradingPair))
            dbQuery = dbQuery.Where(o => o.TradingPair == query.TradingPair);

        if (query.OrdersType == TradesTableOrderType.Closed)
            throw new NotImplementedException(); 
        
        if (query.OrdersType == TradesTableOrderType.Opened)
            throw new NotImplementedException();

        var total = await dbQuery.LongCountAsync();
        var trades = await dbQuery
            .OrderByDescending(t => t.CreatedAt)
            .Skip(query.Page * query.PageSize)
            .Take(query.PageSize)
            .Select(t => new TradesTableItem
            {
                TradeId = t.TradeId,
                TradingPair = t.TradingPair,
                CreatedAt = t.CreatedAt,
                HeldCoinsCount = t.HeldCoinsCount,
                CurrentProfit = t.CurrentProfit,
                Closed = t.Closed,
                Orders = t.History
            })
            .ToListAsync();

        return new PagedData<TradesTableItem>(trades, query.PageSize, query.PageSize, total);
    }
}