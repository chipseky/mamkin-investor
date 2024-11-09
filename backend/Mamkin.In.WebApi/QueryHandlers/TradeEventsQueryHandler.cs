using Mamkin.In.Infrastructure.Ef;
using Mamkin.In.WebApi.Contracts.Paging;
using Mamkin.In.WebApi.Contracts.Req;
using Mamkin.In.WebApi.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Mamkin.In.WebApi.QueryHandlers;

public class TradeEventsQueryHandler
{
    private readonly ApplicationDbContext _dbContext;

    public TradeEventsQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IPagedData<object>> Handle(TradeEventsQuery query)
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