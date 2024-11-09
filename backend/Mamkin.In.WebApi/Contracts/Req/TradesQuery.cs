using Mamkin.In.Domain;
using Mamkin.In.WebApi.Contracts.Paging;

namespace Mamkin.In.WebApi.Contracts.Req;

public class TradesQuery : IPagination
{
    public string TradingPair { get; set; }
    public TradeState? TradeState { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}