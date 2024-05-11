using Chipseky.MamkinInvestor.Domain;
using Chipseky.MamkinInvestor.WebApi.Dtos;
using Chipseky.MamkinInvestor.WebApi.Infrastructure;
using Chipseky.MamkinInvestor.WebApi.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Chipseky.MamkinInvestor.WebApi.Queries;

public class OrdersTableDataQuery : IPagination
{
    public string TradingPair { get; set; }
    public OrdersType OrdersType { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public enum OrdersType
{
    All,
    Opened,
    Closed
}

public class OrdersTableDataQueryHandler
{
    private readonly ApplicationDbContext _dbContext;

    public OrdersTableDataQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IPagedData<Order>> Handle(OrdersTableDataQuery query)
    {
        var dbQuery = _dbContext.Orders.AsNoTracking();

        if (!string.IsNullOrEmpty(query.TradingPair))
            dbQuery = dbQuery.Where(o => o.TradingPair == query.TradingPair);

        if (query.OrdersType == OrdersType.Closed)
            throw new NotImplementedException(); 
        
        if (query.OrdersType == OrdersType.Opened)
            throw new NotImplementedException();

        var total = await dbQuery.LongCountAsync();
        var orders = await dbQuery
            .Skip(query.Page * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PagedData<Order>(orders, query.PageSize, query.PageSize, total);
    }
}