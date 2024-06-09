using Chipseky.MamkinInvestor.WebApi.Infrastructure;
using Chipseky.MamkinInvestor.WebApi.Infrastructure.Database;
using Chipseky.MamkinInvestor.WebApi.Queries;
using Microsoft.EntityFrameworkCore;

namespace Chipseky.MamkinInvestor.WebApi.QueryHandlers;

public class TradeEventsTableDataQueryHandler
{
    private readonly ApplicationDbContext _dbContext;

    public TradeEventsTableDataQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IPagedData<object>> Handle(TradeEventsTableDataQuery query)
    {
        var dbQuery = _dbContext.TradeEvents.AsNoTracking();

        var total = await dbQuery.LongCountAsync();
        var trades = await dbQuery
            .OrderByDescending(te => te.DbTradeEventId)
            .Skip(query.Page * query.PageSize)
            .Take(query.PageSize)
            .Select(te => te.Data)
            .ToListAsync();

        return new PagedData<object>(trades, query.PageSize, query.PageSize, total);
    }
}