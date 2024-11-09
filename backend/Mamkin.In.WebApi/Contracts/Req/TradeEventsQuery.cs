using Mamkin.In.WebApi.Contracts.Paging;

namespace Mamkin.In.WebApi.Contracts.Req;

public class TradeEventsQuery : IPagination
{
    public int Page { get; set; }
    public int PageSize { get; set; }
}