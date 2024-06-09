using Chipseky.MamkinInvestor.WebApi.Dtos;

namespace Chipseky.MamkinInvestor.WebApi.Queries;

public class TradeEventsTableDataQuery : IPagination
{
     public int Page { get; set; }
     public int PageSize { get; set; }
}